namespace DataChannelDotnet.Events;

public sealed unsafe class RtcTextReceivedEvent
{
    public sbyte* Data { get; }
    public int Length { get; }

    internal RtcTextReceivedEvent(sbyte* data, int length)
    {
        Data = data;
        Length = length;
    }
}