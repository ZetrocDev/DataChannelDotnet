using DataChannelDotnet.Events;
using DataChannelDotnet.Data;
using DataChannelDotnet.Bindings;

namespace DataChannelDotnet;

public interface IRtcTrack : IDisposable
{
    bool IsOpen { get; }
    bool IsClosed { get; }
    uint Timestamp { get; set; }
    rtcDirection Direction { get; }
    string? Description { get; }
    string? Mid { get; }

    event Action<IRtcTrack>? OnOpen;
    event Action<IRtcTrack>? OnClose;
    event Action<IRtcTrack, string?>? OnError;
    event Action<IRtcTrack, RtcBinaryReceivedEventSafe>? OnBinaryReceivedSafe;
    
    void AddH264Packetizer(RtcPacketizerInitArgs args);
    void AddH265Packetizer(RtcPacketizerInitArgs args);
    void AddAv1Packetizer(RtcPacketizerInitArgs args);
    void AddOpusPacketizer(RtcPacketizerInitArgs args);
    void AddAacPacketizer(RtcPacketizerInitArgs args);
    void AddPcmuPacketizer(RtcPacketizerInitArgs args);
    void AddPcmaPacketizer(RtcPacketizerInitArgs args);
    void AddG722Packetizer(RtcPacketizerInitArgs args);
    
    void SetPliHandler(Action callback);
    void AddRtcpNackResponder(uint maxPackets);
    void AddRtcpSrReporter();
    
    void Write(ReadOnlySpan<byte> buffer);

    uint ConvertTimestampSecondsToTimestamp(double seconds);
    uint GetLastTrackSenderReportTimestamp();
}