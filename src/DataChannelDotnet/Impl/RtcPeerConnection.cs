using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using DataChannelDotnet.Bindings;
using DataChannelDotnet.Data;
using DataChannelDotnet.Events;
using DataChannelDotnet.Internal;

namespace DataChannelDotnet.Impl;

public sealed class RtcPeerConnection : IRtcPeerConnection
{
    public event Action<IRtcPeerConnection, rtcState>? OnConnectionStateChange;
    public event Action<IRtcPeerConnection, rtcGatheringState>? OnGatheringStateChange;
    public event Action<IRtcPeerConnection, rtcSignalingState>? OnSignalingStateChange;
    public event Action<IRtcPeerConnection, rtcIceState>? OnIceStateChange;
    public event Action<IRtcPeerConnection, IRtcDataChannel>? OnDataChannel;
    public event Action<IRtcPeerConnection, IRtcTrack>? OnTrack;
    public event Action<IRtcPeerConnection, RtcPeerCandidateEvent>? OnCandidate;
    public event Action<IRtcPeerConnection, RtcCandidate>? OnCandidateSafe;
    public event Action<IRtcPeerConnection, RtcLocalDescriptionEvent>? OnLocalDescription;
    public event Action<IRtcPeerConnection, RtcDescription>? OnLocalDescriptionSafe;

    public unsafe string LocalDescription => RtcHelpers.GetString(_peerId, Rtc.rtcGetLocalDescription, _lock, ref _disposed, nameof(RtcPeerConnection));
    public RtcDescriptionType LocalDescriptionType => GetLocalDescriptionType();
    public unsafe string RemoteDescription => RtcHelpers.GetString(_peerId, Rtc.rtcGetLocalDescription, _lock, ref _disposed, nameof(RtcPeerConnection));
    public RtcDescriptionType RemoteDescriptionType => GetRemoteDescriptionType();
    public unsafe string LocalAddress => RtcHelpers.GetString(_peerId, Rtc.rtcGetLocalAddress, _lock, ref _disposed, nameof(RtcPeerConnection));
    public unsafe string RemoteAddress => RtcHelpers.GetString(_peerId, Rtc.rtcGetRemoteAddress, _lock, ref _disposed, nameof(RtcPeerConnection));
    public bool NegotiationRequired => GetNegotiationNeeded();
    public rtcState ConnectionState { get; private set; }
    public rtcGatheringState GatheringState { get; private set; }
    public rtcSignalingState SignalingState { get; private set; }
    public rtcIceState IceState { get; private set; }

    private readonly int _peerId;
    private readonly GCHandle _thisHandle;
    private readonly Lock _lock = new();

    private readonly List<RtcDataChannel> _dataChannels = new List<RtcDataChannel>();
    private readonly List<RtcTrack> _tracks = new List<RtcTrack>();

    private bool _disposed;
    private bool _closeEventRaised;

    public RtcPeerConnection(RtcPeerConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        try
        {
            _thisHandle = GCHandle.Alloc(this);
            _peerId = CreateRtcPeer(configuration);

            unsafe
            {
                Rtc.rtcSetUserPointer(_peerId, (void*)GCHandle.ToIntPtr(_thisHandle));

                Rtc.rtcSetStateChangeCallback(_peerId, &StaticOnConnectionStateCallback).GetValueOrThrow();
                Rtc.rtcSetSignalingStateChangeCallback(_peerId, &StaticOnSignalingStateCallback).GetValueOrThrow();
                Rtc.rtcSetIceStateChangeCallback(_peerId, &StaticOnIceStateChangeCallback).GetValueOrThrow();
                Rtc.rtcSetGatheringStateChangeCallback(_peerId, &StaticOnGatheringStateCallback).GetValueOrThrow();
                
                Rtc.rtcSetLocalCandidateCallback(_peerId, &StaticOnLocalCandidateCallback).GetValueOrThrow();
                Rtc.rtcSetLocalDescriptionCallback(_peerId, &StaticOnLocalDescriptionCallback).GetValueOrThrow();
                Rtc.rtcSetDataChannelCallback(_peerId, &StaticOnDataChannelCallback).GetValueOrThrow();
                Rtc.rtcSetTrackCallback(_peerId, &StaticOnTrackCallback).GetValueOrThrow();
            }
        }
        catch (Exception)
        {
            Dispose();
            throw;
        }
    }

    private unsafe int CreateRtcPeer(RtcPeerConfiguration configuration)
    {
        using NativeUtf8StringArray iceServersArray =
            new NativeUtf8StringArray(configuration.IceServers?.ToArray() ?? []);
        using NativeUtf8String proxyServerStr = new NativeUtf8String(configuration.ProxyServer);
        using NativeUtf8String bindAddressStr = new NativeUtf8String(configuration.BindAddress);

        rtcConfiguration nativeConf = new rtcConfiguration()
        {
            iceServers = iceServersArray.Ptr,
            iceServersCount = iceServersArray.StringCount,
            proxyServer = proxyServerStr.Ptr,
            bindAddress = bindAddressStr.Ptr,
            certificateType = configuration.CertificateType,
            disableAutoNegotiation = configuration.DisableAutoNegotiation ? (byte)1 : (byte)0,
            enableIceTcp = configuration.EnableIceTcp ? (byte)1 : (byte)0,
            enableIceUdpMux = configuration.EnableIceUdpMux ? (byte)1 : (byte)0,
            forceMediaTransport = configuration.ForceMediaTransport ? (byte)1 : (byte)0,
            iceTransportPolicy = configuration.TransportPolicy,
            maxMessageSize = configuration.MaxMessageSize,
            mtu = configuration.Mtu,
            portRangeBegin = configuration.PortRangeBegin,
            portRangeEnd = configuration.PortRangeEnd
        };

        return Rtc.rtcCreatePeerConnection(&nativeConf).GetValueOrThrow();
    }

    public unsafe IRtcDataChannel CreateDataChannel(RtcCreateDataChannelArgs args)
    {
        using (_lock.EnterScope())
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(RtcPeerConnection));

            using var label = new NativeUtf8String(args.Label);
            using var protocol =
                new NativeUtf8String(args.Protocol == RtcDataChannelProtocol.Binary ? "binary" : "text");

            rtcDataChannelInit nativeArgs = new()
            {
                manualStream = (byte)(args.ManualStream ? 1 : 0),
                negotiated = (byte)(args.Negotiated ? 1 : 0),
                protocol = protocol.Ptr,
                stream = args.Stream,
                reliability = new rtcReliability()
                {
                    maxPacketLifeTime = args.MaxPacketLifetime,
                    maxRetransmits = args.MaxRetransmits,
                    unordered = (byte)(args.Unordered ? 1 : 0),
                    unreliable = (byte)(args.Unreliable ? 1 : 0)
                }
            };

            int track = Rtc.rtcCreateDataChannelEx(_peerId, label.Ptr, &nativeArgs).GetValueOrThrow();
            RtcDataChannel channel = new RtcDataChannel(track);
            _dataChannels.Add(channel);
            return channel;
        }
    }

    public unsafe IRtcTrack CreateTrack(RtcCreateTrackArgs args)
    {
        using (_lock.EnterScope())
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(RtcPeerConnection));

            using NativeUtf8String trackId = new NativeUtf8String(args.TrackId);
            using NativeUtf8String mid = new NativeUtf8String(args.Mid);
            using NativeUtf8String msid = new NativeUtf8String(args.Msid);
            using NativeUtf8String name = new NativeUtf8String(args.Name);
            using NativeUtf8String profile = new NativeUtf8String(args.Profile);

            rtcTrackInit initArgs = new rtcTrackInit
            {
                trackId = trackId.Ptr,
                codec = args.Codec,
                direction = args.Direction,
                mid = mid.Ptr,
                msid = msid.Ptr,
                name = name.Ptr,
                payloadType = args.PayloadType,
                profile = profile.Ptr,
                ssrc = args.Ssrc
            };

            int tr = Rtc.rtcAddTrackEx(_peerId, &initArgs).GetValueOrThrow();
            RtcTrack track = new RtcTrack(tr);
            _tracks.Add(track);
            return track;
        }
    }


    public unsafe void AddRemoteCandidate(RtcCandidate candidate)
    {
        using (_lock.EnterScope())
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(RtcPeerConnection));

            NativeUtf8String candidateStr = new NativeUtf8String(candidate.Content);
            NativeUtf8String midStr = new NativeUtf8String(candidate.Mid);

            Rtc.rtcAddRemoteCandidate(_peerId, candidateStr.Ptr, midStr.Ptr).GetValueOrThrow();
        }
    }

    public unsafe void SetRemoteDescription(RtcDescription description)
    {
        using (_lock.EnterScope())
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(RtcPeerConnection));

            ReadOnlySpan<byte> utf8Text;

            if (description.Type == RtcDescriptionType.Answer)
                utf8Text = "answer\0"u8;
            else if (description.Type == RtcDescriptionType.Offer)
                utf8Text = "offer\0"u8;
            else if (description.Type == RtcDescriptionType.PrAnswer)
                utf8Text = "pranswer\0"u8;
            else if (description.Type == RtcDescriptionType.Rollback)
                utf8Text = "rollback\0"u8;
            else
                throw new ArgumentException("Invalid type: " + nameof(description.Type));

            using NativeUtf8String sdpString = new NativeUtf8String(description.Sdp);

            fixed (byte* typeStr = utf8Text)
            {
                Rtc.rtcSetRemoteDescription(_peerId, sdpString.Ptr, (sbyte*)typeStr).GetValueOrThrow();
            }
        }
    }

    public unsafe void SetLocalDescription(RtcDescriptionType type)
    {
        using (_lock.EnterScope())
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(RtcPeerConnection));

            ReadOnlySpan<byte> utf8Text;

            if (type == RtcDescriptionType.Answer)
                utf8Text = "answer\0"u8;
            else if (type == RtcDescriptionType.Offer)
                utf8Text = "offer\0"u8;
            else if (type == RtcDescriptionType.PrAnswer)
                utf8Text = "pranswer\0"u8;
            else if (type == RtcDescriptionType.Rollback)
                utf8Text = "rollback\0"u8;
            else
                throw new ArgumentException("Invalid type: " + nameof(type));


            fixed (byte* textPtr = utf8Text)
            {
                Rtc.rtcSetLocalDescription(_peerId, (sbyte*)textPtr).GetValueOrThrow();
            }
        }
    }

    public bool TryGetSelectedCandidatePair([NotNullWhen(true)] out RtcCandidatePair? pair)
        => RtcHelpers.TryGetSelectedCandidatePair(_peerId, _lock, ref _disposed, out pair);
    
    private unsafe RtcDescriptionType GetRemoteDescriptionType()
    {
        using var _ = _lock.EnterScope();
        ObjectDisposedException.ThrowIf(_disposed, nameof(RtcPeerConnection));

        Span<byte> buffer = stackalloc byte[16];

        fixed (byte* bufferPtr = buffer)
        {
            Rtc.rtcGetRemoteDescriptionType(_peerId, (sbyte*)bufferPtr, 16).GetValueOrThrow();
            return RtcHelpers.ParseSdpType((sbyte*)bufferPtr);
        }
    }
    
    private unsafe RtcDescriptionType GetLocalDescriptionType()
    {
        using var _ = _lock.EnterScope();
        ObjectDisposedException.ThrowIf(_disposed, nameof(RtcPeerConnection));

        Span<byte> buffer = stackalloc byte[16];

        fixed (byte* bufferPtr = buffer)
        {
            Rtc.rtcGetLocalDescriptionType(_peerId, (sbyte*)bufferPtr, 16).GetValueOrThrow();
            return RtcHelpers.ParseSdpType((sbyte*)bufferPtr);
        }
    }

    private bool GetNegotiationNeeded()
    {
        using var _ = _lock.EnterScope();
        ObjectDisposedException.ThrowIf(_disposed, nameof(RtcPeerConnection));
        return Rtc.rtcIsNegotiationNeeded(_peerId) > 0;
    }

    #region Callbacks

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnGatheringStateCallback(int id, rtcGatheringState state, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcPeerConnection? instance))
                return;

            instance.GatheringState = state;
            var cb = instance.OnGatheringStateChange;
            cb?.Invoke(instance, state);
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnIceStateChangeCallback(int id, rtcIceState state, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcPeerConnection? instance))
                return;

            instance.IceState = state;
            var cb = instance.OnIceStateChange;
            cb?.Invoke(instance, state);
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnSignalingStateCallback(int id, rtcSignalingState state, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcPeerConnection? instance))
                return;

            instance.SignalingState = state;
            var cb = instance.OnSignalingStateChange;
            cb?.Invoke(instance, state);
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnTrackCallback(int id, int trackId, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcPeerConnection? instance))
                return;

            RtcTrack track = new RtcTrack(trackId);
            instance._tracks.Add(track);

            var cb = instance.OnTrack;
            cb?.Invoke(instance, track);
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnDataChannelCallback(int id, int channelId, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcPeerConnection? instance))
                return;

            RtcDataChannel dc = new RtcDataChannel(channelId);
            instance._dataChannels.Add(dc);

            var cb = instance.OnDataChannel;
            cb?.Invoke(instance, dc);
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnLocalDescriptionCallback(int id, sbyte* sdpStr, sbyte* typeStr, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcPeerConnection? instance))
                return;

            Action<IRtcPeerConnection, RtcLocalDescriptionEvent>? unsafeCallback = instance.OnLocalDescription;
            unsafeCallback?.Invoke(instance, new RtcLocalDescriptionEvent(sdpStr, typeStr));

            Action<IRtcPeerConnection, RtcDescription>? safeCallback = instance.OnLocalDescriptionSafe;

            if (safeCallback is not null)
            {
                RtcDescriptionType type = RtcHelpers.ParseSdpType(typeStr);

                string sdp = Utf8StringMarshaller.ConvertToManaged((byte*)sdpStr)!;
                safeCallback(instance, new RtcDescription { Sdp = sdp, Type = type });
            }
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnLocalCandidateCallback(int id, sbyte* candidate, sbyte* mid, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcPeerConnection? instance))
                return;

            Action<IRtcPeerConnection, RtcPeerCandidateEvent>? unsafeCallback = instance.OnCandidate;

            if (unsafeCallback is not null)
                unsafeCallback(instance, new RtcPeerCandidateEvent(candidate, mid));

            Action<IRtcPeerConnection, RtcCandidate>? safeCallback = instance.OnCandidateSafe;

            if (safeCallback is not null)
                safeCallback(instance,
                    new RtcCandidate
                    {
                        Mid = Utf8StringMarshaller.ConvertToManaged((byte*)mid)!,
                        Content = Utf8StringMarshaller.ConvertToManaged((byte*)candidate)!
                    });
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticOnConnectionStateCallback(int id, rtcState connectionState, void* user)
    {
        try
        {
            if (!RtcThread.TryGetRtcObjectInstance(user, out RtcPeerConnection? instance))
                return;

            instance.ConnectionState = connectionState;
            Action<IRtcPeerConnection, rtcState>? callback = instance.OnConnectionStateChange;

            if (connectionState == rtcState.RTC_CLOSED)
            {
                instance._closeEventRaised = true;
            }

            callback?.Invoke(instance, connectionState);
        }
        catch (Exception ex)
        {
            RtcTools.RaiseUnhandledException(ex);
        }
    }

    #endregion

    private void Cleanup()
    {
        foreach (var channel in _dataChannels)
            channel.Dispose();

        foreach (var track in _tracks)
            track.Dispose();

        _dataChannels.Clear();
        _tracks.Clear();
        Rtc.rtcClosePeerConnection(_peerId);
        //The connection events are not instantly raised, waiting a tiny amount
        //seems to ensure that they actually get raised.
        //rtcDeletePeerConnection should then block until all callbacks are complete.
        WaitForCallbacks();
        Rtc.rtcDeletePeerConnection(_peerId);

        ConnectionState = rtcState.RTC_CLOSED;
        SignalingState = rtcSignalingState.RTC_SIGNALING_STABLE;
        IceState = rtcIceState.RTC_ICE_CLOSED;
        GatheringState = rtcGatheringState.RTC_GATHERING_COMPLETE;

        _thisHandle.Free();
    }

    private void WaitForCallbacks()
    {
        for (int i = 0; i < 100; i++)
        {
            if (_closeEventRaised)
                return;

            Thread.Sleep(25);
        }

        Console.WriteLine("!! peer did not close properly");
    }

    public void Dispose()
    {
        using (_lock.EnterScope())
        {
            if (_disposed)
                return;

            _disposed = true;

            //rtcDeletePeerConnections will block until all scheduled callbacks have executed, which will cause a deadlock
            //so we should use another thread and release the lock to allow other callbacks to complete.
            if (RtcThread.IsRtcThread)
                Task.Run(Cleanup);
            else
                Cleanup();
        }
    }
}