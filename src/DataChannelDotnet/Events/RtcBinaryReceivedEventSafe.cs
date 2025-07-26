namespace DataChannelDotnet.Events;

public readonly ref struct RtcBinaryReceivedEventSafe
{
    public ReadOnlySpan<byte> Data { get; }

    internal RtcBinaryReceivedEventSafe(ReadOnlySpan<byte> data)
    {
        Data = data;
    }
}