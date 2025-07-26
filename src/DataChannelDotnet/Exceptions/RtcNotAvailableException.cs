namespace DataChannelDotnet.Exceptions;

internal sealed class RtcNotAvailableException : RtcException
{
    public RtcNotAvailableException() : base("Unavailable")
    {
    }
}