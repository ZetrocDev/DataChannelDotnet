using DataChannelDotnet.Bindings;
using DataChannelDotnet.Internal;

namespace DataChannelDotnet;

public static class RtcTools
{
    public static Action<Exception>? OnUnhandledException;
    
    public static void Preload() => Rtc.rtcPreload();
    public static void Cleanup() => Rtc.rtcCleanup();

    public static void SetSctpSettings(rtcSctpSettings settings)
    {
        unsafe
        {
            Rtc.rtcSetSctpSettings(&settings).GetValueOrThrow();
        }
    }

    internal static void RaiseUnhandledException(Exception ex)
    {
        var cb = OnUnhandledException;
        cb?.Invoke(ex);
    }
}