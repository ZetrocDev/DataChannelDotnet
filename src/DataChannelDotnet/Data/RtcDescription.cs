namespace DataChannelDotnet.Data;

public sealed class RtcDescription
{
    public required string Sdp { get; init; }
    public required RtcDescriptionType Type { get; init; }
}