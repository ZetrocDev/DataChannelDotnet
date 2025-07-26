using DataChannelDotnet.Bindings;

namespace DataChannelDotnet.Data;

public sealed class RtcCreateTrackArgs
{
    public rtcDirection Direction { get; set; }
    public rtcCodec Codec { get; set; }
    public int PayloadType { get; set; }
    public uint Ssrc { get; set; }
    public string? Mid { get; set; }
    public string? Name { get; set; }
    public string? Msid { get; set; }
    public string? TrackId { get; set; }
    public string? Profile { get; set; }
}