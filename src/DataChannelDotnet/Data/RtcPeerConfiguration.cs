using DataChannelDotnet.Bindings;

namespace DataChannelDotnet.Data;

public sealed class RtcPeerConfiguration
{
    public ICollection<string>? IceServers { get; set; }
    public string? ProxyServer { get; set; }
    public string? BindAddress { get; set; }
    public rtcCertificateType CertificateType { get; set; }
    public rtcTransportPolicy TransportPolicy { get; set; }
    public bool EnableIceTcp { get; set; }
    public bool EnableIceUdpMux { get; set; }
    public bool DisableAutoNegotiation { get; set; }
    public bool ForceMediaTransport { get; set; }
    public ushort PortRangeBegin { get; set; }
    public ushort PortRangeEnd { get; set; }
    public int Mtu { get; set; }
    public int MaxMessageSize { get; set; }
}