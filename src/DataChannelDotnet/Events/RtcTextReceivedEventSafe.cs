namespace DataChannelDotnet.Events;

public readonly ref struct RtcTextReceivedEventSafe
{
    public string Text { get; }

    public RtcTextReceivedEventSafe(string text)
    {
        Text = text;
    }
}