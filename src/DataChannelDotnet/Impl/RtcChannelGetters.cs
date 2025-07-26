using DataChannelDotnet.Bindings;

namespace DataChannelDotnet.Impl;

internal static class RtcChannelGetters
{
    public static bool GetIsOpen(int id, Lock @lock, ref bool disposed)
    {
        using (@lock.EnterScope())
        {
            if (disposed)
                return false;

            return Rtc.rtcIsOpen(id) > 0;
        }
    }

    public static bool GetIsClosed(int id, Lock @lock, ref bool disposed)
    {
        using (@lock.EnterScope())
        {
            if (disposed)
                return true;

            return Rtc.rtcIsClosed(id) > 0;
        }
    }
}