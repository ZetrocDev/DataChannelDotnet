using DataChannelDotnet.Exceptions;

namespace DataChannelDotnet.Internal;

internal static class Extensions
{
    /// <summary>
    /// Returns the value if it is not an RTC error code, otherwise throws an exception based on the error code.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="RtcInvalidArgumentException"></exception>
    /// <exception cref="RtcFailureException"></exception>
    /// <exception cref="RtcNotAvailableException"></exception>
    /// <exception cref="RtcBufferTooSmallException"></exception>
    public static int GetValueOrThrow(this int value)
    {
        if (value == -1)
            throw new RtcInvalidArgumentException();
        else if (value == -2)
            throw new RtcFailureException();
        else if (value == -3)
            throw new RtcNotAvailableException();
        else if (value == -4)
            throw new RtcBufferTooSmallException();

        return value;
    }
}
