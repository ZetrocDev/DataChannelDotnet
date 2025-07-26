using DataChannelDotnet.Bindings;

namespace DataChannelDotnet.Data;

public sealed class RtcPacketizerInitArgs
{
    public int Ssrc { get; set; }
    public string? Cname { get; set; }
    public byte PayloadType { get; set; }
    public uint Clockrate { get; set; }
    public ushort SequenceNumber { get; set; }
    public uint Timestamp { get; set; }
    public ushort MaxFragmentSize { get; set; }
    public rtcNalUnitSeparator NalUnitSeparator { get; set; }
    public rtcObuPacketization ObuPacketization { get; set; }
    public byte PlayoutDelayId { get; set; }
    public ushort PlayoutDelayMin { get; set; }
    public ushort PlayoutDelayMax { get; set; }
}