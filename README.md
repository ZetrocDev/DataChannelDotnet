# DataChannelDotnet - C# WebRtc library
DataChannelDotnet is a high performance, minimal overhead wrapper around [libdatachannel](https://github.com/paullouisageneau/libdatachannel) for WebRtc, it supports both datachannels and media tracks. Raw bindings are generated automatically via [clangsharp](https://github.com/dotnet/ClangSharp) using Github Actions, and are included in the DataChannel.Bindings package.


### Usage (Managed version)

```
IRtcPeerConnection host = new RtcPeerConnection(new RtcPeerConfiguration()
{
    IceServers = ["stun:stun.l.google.com:19302"]
});

IRtcPeerConnection client = new RtcPeerConnection(new RtcPeerConfiguration()
{
    IceServers = ["stun:stun.l.google.com:19302"]
});

host.OnConnectionStateChange += (_, state) => Console.WriteLine($"Host connection state -> {state}");
client.OnConnectionStateChange += (_, state) => Console.WriteLine($"Client connection state -> {state}");

host.OnCandidateSafe += (_, candidate) => client.AddRemoteCandidate(candidate);
client.OnCandidateSafe += (_, candidate) => host.AddRemoteCandidate(candidate);

host.OnLocalDescriptionSafe += (_, description) => client.SetRemoteDescription(description);
client.OnLocalDescriptionSafe += (_, description) => host.SetRemoteDescription(description);

client.OnDataChannel += (_, channel) =>
{
    channel.OnTextReceivedSafe += (dataChannel, data) =>
    {
        Console.WriteLine($"Client: received '{data.Text}'");
    };
};

var hostChannel = host.CreateDataChannel(new RtcCreateDataChannelArgs()
{
    Label = "testchannel1"
});

hostChannel.OnOpen += channel =>
{
    channel.Send("Hello :)");
};
```

Raw C bindings can also be used via the static Rtc class in DataChannel.Bindings. Check the [C API Documentation](https://github.com/paullouisageneau/libdatachannel/blob/master/DOC.md) from Libdatachannel for usage. Note that using the raw bindings will require unsafe code to be enabled in your project.
