namespace DataChannelDotnet.Exceptions;

internal abstract class RtcException : Exception
{
    protected RtcException(string message) : base(message)
    {

    }
}
