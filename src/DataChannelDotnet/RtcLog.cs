using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DataChannelDotnet.Bindings;

namespace DataChannelDotnet;

public delegate void RtcLogCallback(rtcLogLevel level, string message);

public static class RtcLog
{
    private static RtcLogCallback? _currentCallback;

    private static Lock _lock = new();
    private static bool _initializeCalled;

    public static void Initialize(rtcLogLevel level, RtcLogCallback callback)
    {
        using (_lock.EnterScope())
        {
            if (_initializeCalled)
                return;

            unsafe
            {
                _currentCallback = callback;
                Rtc.rtcInitLogger(level, &StaticLogCallback);
            }

            _initializeCalled = true;
        }
    }

    public static void ChangeCallback(RtcLogCallback? callback)
    {
        _currentCallback = callback;
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void StaticLogCallback(rtcLogLevel level, sbyte* message)
    {
        RtcLogCallback? callback = _currentCallback;
        callback?.Invoke(level, Marshal.PtrToStringAnsi((nint)message) ?? string.Empty);
    }
}