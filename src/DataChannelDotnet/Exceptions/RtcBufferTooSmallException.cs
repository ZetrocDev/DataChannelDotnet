namespace DataChannelDotnet.Exceptions;

internal sealed class RtcBufferTooSmallException : RtcException
{
    public RtcBufferTooSmallException() : base("Buffer was too small")
    {
    }
}