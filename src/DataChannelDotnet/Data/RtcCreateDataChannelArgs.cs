namespace DataChannelDotnet.Data;

public sealed class RtcCreateDataChannelArgs
{
    public required string Label { get; set; }
    public bool Unordered { get; set; }
    public bool Unreliable { get; set; }
    public uint MaxPacketLifetime { get; set; }
    public uint MaxRetransmits { get; set; }

    public RtcDataChannelProtocol Protocol { get; set; }
    public bool Negotiated { get; set; }
    public bool ManualStream { get; set; }
    public ushort Stream { get; set; }
}