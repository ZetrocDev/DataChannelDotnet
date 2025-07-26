namespace DataChannelDotnet.Exceptions;

internal sealed class RtcFailureException : RtcException
{
    public RtcFailureException() : base("Unknown Rtc failure")
    {

    }
}