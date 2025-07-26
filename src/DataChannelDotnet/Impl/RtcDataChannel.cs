using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using DataChannelDotnet.Bindings;
using DataChannelDotnet.Events;
using DataChannelDotnet.Internal;

namespace DataChannelDotnet.Impl;

public sealed class RtcDataChannel : IRtcDataChannel
{
    public bool IsOpen => RtcChannelGetters.GetIsOpen(_channelId, _lock, ref _disposed);
    public bool IsClosed => RtcChannelGetters.GetIsClosed(_channelId, _lock, ref _disposed);
    public string Label => GetLabel();
    
    public event Action<IRtcDataChannel, RtcTextReceivedEvent>? OnTextReceived;
    public event Action<IRtcDataChannel, RtcTextReceivedEventSafe>? OnTextReceivedSafe;
    public event Action<IRtcDataChannel, RtcBinaryReceivedEventSafe>? OnBinaryReceivedSafe;
    public event Action<IRtcDataChannel>? OnOpen;
    public event Action<IRtcDataChannel>? OnClose;
    public event Action<IRtcDataChannel, string?>? OnError;
    
    
    private readonly int _channelId;
    private readonly Lock _lock = new();
    
    private bool _disposed;
    private string? _label;
    private GCHandle _thisHandle;

    internal unsafe RtcDataChannel(int channelId)
    {
        _channelId = channelId;

        try
        {
            _thisHandle = GCHandle.Alloc(this);
            Rtc.rtcSetUserPointer(_channelId, (void*)GCHandle.ToIntPtr(_thisHandle));
            Rtc.rtcSetOpenCallback(_channelId, &StaticOnOpenedCallback).GetValueOrThrow();
            Rtc.rtcSetMessageCallback(_channelId, &StaticOnMessageCallback).GetValueOrThrow();
            Rtc.rtcSetClosedCallback(_channelId, &StaticOnClosedCallback).GetValueOrThrow();
            Rtc.rtcSetErrorCallback(_channelId, &StaticOnErrorCallback).GetValueOrThrow();
        }
        catch (Exception)
        {
            Dispose();
            throw;
        }
    }

    public unsafe void Send(ReadOnlySpan<byte> buffer)
    {
        using (_lock.EnterScope())
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(RtcDataChannel));

            fixed (byte* bufferPtr = buffer)
            {
                Rtc.rtcSendMessage(_channelId, (sbyte*)bufferPtr, buffer.Length).GetValueOrThrow();
            }
        }
    }

    public unsafe void Send(string text)
    {
        using (_lock.EnterScope())
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(RtcDataChannel));

            using NativeUtf8String nativeUtf8String = new NativeUtf8String(text + '\0');

            Rtc.rtcSendMessage(_channelId, nativeUtf8String.Ptr, -1).GetValueOrThrow();
        }
    }

    private unsafe string GetLabel()
    {
        using (_lock.EnterScope())
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(RtcDataChannel));

            if (_label is null)
            {
                Span<byte> buffer = stackalloc byte[128];

                fixed (byte* buffePtr = buffer)
                {
                    Rtc.rtcGetDataChannelLabel(_channelId, (sbyte*)buffePtr, 128).GetValueOrThrow();
                    _label = Utf8StringMarshaller.ConvertToManaged(buffePtr);
                }
            }

            return _label ?? string.Empty;
        }
    }

    #region Callbacks

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnErrorCallback(int id, sbyte* buffer, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcDataChannel? instance))
                return;

            var cb = instance.OnError;

            if (instance._disposed || cb is null)
                return;

            string errorStr = Utf8StringMarshaller.ConvertToManaged((byte*)buffer) ?? string.Empty;

            cb.Invoke(instance, errorStr);
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnClosedCallback(int id, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcDataChannel? instance))
                return;

            var cb = instance.OnClose;
            
            if (instance._disposed || cb is null)
                return;

            cb.Invoke(instance);
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnOpenedCallback(int id, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcDataChannel? instance))
                return;
            
            var cb = instance.OnOpen;
            cb?.Invoke(instance);
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnMessageCallback(int id, sbyte* buffer, int size, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcDataChannel? instance))
                return;

            if (size > 0)
                StaticHandleBinaryMessage(instance, buffer, size);
            else if (size < 0)
                StaticHandleTextMessage(instance, buffer, size);
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    private static unsafe void StaticHandleBinaryMessage(RtcDataChannel channel, sbyte* buffer, int size)
    {
        Action<IRtcDataChannel, RtcBinaryReceivedEventSafe>? callback = channel.OnBinaryReceivedSafe;
        callback?.Invoke(channel, new RtcBinaryReceivedEventSafe(new ReadOnlySpan<byte>(buffer, size)));
    }

    private static unsafe void StaticHandleTextMessage(RtcDataChannel channel, sbyte* buffer, int size)
    {
        Action<IRtcDataChannel, RtcTextReceivedEventSafe>? safeCallback = channel.OnTextReceivedSafe;

        if (safeCallback is not null)
        {
            string? text = Utf8StringMarshaller.ConvertToManaged((byte*)buffer);

            if (text is not null)
                safeCallback(channel, new RtcTextReceivedEventSafe(text));
        }

        Action<IRtcDataChannel, RtcTextReceivedEvent>? unsafeCallback = channel.OnTextReceived;
        unsafeCallback?.Invoke(channel, new RtcTextReceivedEvent(buffer, -size));
    }

    #endregion

    private void Cleanup()
    {
        Rtc.rtcClose(_channelId);
        WaitForCallbacks();
        Rtc.rtcDeleteDataChannel(_channelId);
        _thisHandle.Free();
    }

    private void WaitForCallbacks()
    {
        for (int i = 0; i < 100; i++)
        {
            if (!IsOpen)
                return;

            Thread.Sleep(25);
        }

        Console.WriteLine("!! RtcDataChannel did not close properly");
    }

    public void Dispose()
    {
        using (_lock.EnterScope())
        {
            if (_disposed)
                return;

            _disposed = true;

            if (RtcThread.IsRtcThread)
                Task.Run(Cleanup);
            else
                Cleanup();
        }
    }
}