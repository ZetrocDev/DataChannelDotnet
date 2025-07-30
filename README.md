# DataChannelDotnet - C# WebRtc library

[![Run .NET Tests](https://github.com/ZetrocDev/DataChannelDotnet/actions/workflows/Run-tests.yml/badge.svg)](https://github.com/ZetrocDev/DataChannelDotnet/actions/workflows/Run-tests.yml)
[![Update libdatachannel](https://github.com/ZetrocDev/DataChannelDotnet/actions/workflows/Update-Bindings.yml/badge.svg)](https://github.com/ZetrocDev/DataChannelDotnet/actions/workflows/Update-Bindings.yml)

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

## Performance
This is a thin wrapper, so the performance should be very close to the native Libdatachannel library. Sending & receiving data (via Track or Datachannel) is entirely allocation free. 

Here is the results of a [throughput benchmark](https://github.com/ZetrocDev/DataChannelDotnet/blob/main/Benchmark/Program.cs), sending & receiving data on localhost, running on .net 9 on a Ryzen 5500. This shows around 55-60MB/s throughput, which is consistent with [Libdatachannels own benchmark](https://github.com/paullouisageneau/libdatachannel/tree/master/examples/client-benchmark). This also shows zero allocation sending & receiving. It should be noted that WebRtc is generally not designed for large file transfer scenarios.

| Method              | Mean         | Error      | StdDev     | Allocated |
|-------------------- |-------------:|-----------:|-----------:|----------:|
| SendAndReceive100MB | 1,728.885 ms | 12.9256 ms | 12.0906 ms |         - |
| SendAndReceive50MB  |   849.148 ms | 11.5134 ms | 10.7696 ms |         - |
| SendAndReceive1MB   |    16.517 ms |  0.3237 ms |  0.3854 ms |         - |
| SendAndReceive256KB |     3.818 ms |  0.1247 ms |  0.3657 ms |         - |