using DataChannelDotnet.Data;

namespace DataChannelDotnet.Events;

public sealed class RtcLocalDescriptionEventSafe
{
    public string? Sdp { get; }
    public RtcDescriptionType Type { get; }

    public RtcLocalDescriptionEventSafe(string? sdp, RtcDescriptionType type)
    {
        Sdp = sdp;
        Type = type;
    }
}