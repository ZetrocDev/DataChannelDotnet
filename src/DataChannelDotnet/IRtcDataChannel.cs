using DataChannelDotnet.Events;

namespace DataChannelDotnet;

public interface IRtcDataChannel : IDisposable
{
    bool IsOpen { get; }
    bool IsClosed { get; }
    
    string Label { get; }

    event Action<IRtcDataChannel>? OnOpen;
    event Action<IRtcDataChannel>? OnClose;
    event Action<IRtcDataChannel, string?>? OnError;
    
    event Action<IRtcDataChannel, RtcTextReceivedEvent>? OnTextReceived;
    event Action<IRtcDataChannel, RtcTextReceivedEventSafe>? OnTextReceivedSafe;
    event Action<IRtcDataChannel, RtcBinaryReceivedEventSafe>? OnBinaryReceivedSafe;

    
    void Send(ReadOnlySpan<byte> buffer);
    void Send(string text);
}