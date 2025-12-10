using DataChannelDotnet.Bindings;
using DataChannelDotnet.Data;
using DataChannelDotnet.Impl;
using System.Text;
using Xunit.Abstractions;
// ReSharper disable AccessToDisposedClosure

namespace DataChannelDotnet.Tests;

public class ConnectionTest
{
    private readonly ITestOutputHelper _output;

    public ConnectionTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [Fact]
    //Connect
    //Send & receive binary
    //send & receive text
    //close
    public void TestPeerConnections()
    {
        RtcTools.OnUnhandledException += exception =>
        {
            _output.WriteLine($"Error {exception}");
            Assert.Fail($"Unhandled exception on RTC callback thread: {exception}");
        };
        
        var host = new RtcPeerConnection(new RtcPeerConfiguration());
        var client = new RtcPeerConnection(new RtcPeerConfiguration());
        
        const string textString = "I'll have two number 9's, a number 9 large, a number 6 with extra dip, a number 7, two number 45's, one with cheese, and a large soda.";
        byte[] testBinary = Encoding.Unicode.GetBytes(textString);

        bool clientConnected = default, hostConnected = default;
        bool clientClosed = false, hostClosed = false;
        Lock syncLock = new();

        using ManualResetEventSlim connectedEvent = new ManualResetEventSlim();
        using ManualResetEventSlim binaryReceivedEvent = new  ManualResetEventSlim();
        using ManualResetEventSlim textReceivedEvent = new ManualResetEventSlim();
        using ManualResetEventSlim closedEvent = new ManualResetEventSlim();

        using ManualResetEventSlim channelOpenedEvent = new ManualResetEventSlim();
        using ManualResetEventSlim channelClosedEvent = new ManualResetEventSlim();

        host.OnConnectionStateChange += (_, state) =>
        {
            _output.WriteLine($"Host connection state -> {state}");
            
            using (syncLock.EnterScope())
            {
                hostConnected = state == rtcState.RTC_CONNECTED;
                hostClosed = state == rtcState.RTC_CLOSED;
            
                if (clientConnected && hostConnected)
                    connectedEvent.Set();
                else if (clientClosed && hostClosed)
                    closedEvent.Set();
            }
        };
        
        client.OnConnectionStateChange += (_, state) =>
        {
            _output.WriteLine($"Client connection state -> {state}");
            
            using (syncLock.EnterScope())
            {
                clientConnected = state == rtcState.RTC_CONNECTED;
                clientClosed = state == rtcState.RTC_CLOSED;

                if (clientConnected && hostConnected)
                    connectedEvent.Set();
                else if (clientClosed && hostClosed)
                    closedEvent.Set();
            }
        };

        host.OnCandidateSafe += (_, cnd) =>
        {
            _output.WriteLine("Host -> sending candidate to client");

            client.AddRemoteCandidate(cnd);
        };
        client.OnCandidateSafe += (_, cnd) =>
        {
            _output.WriteLine("Client -> sending candidate to host");
            host.AddRemoteCandidate(cnd);
        };

        host.OnLocalDescriptionSafe += (_, desc) =>
        {
            _output.WriteLine("Host description changed to type " + desc.Type);
            
            if (desc.Type == RtcDescriptionType.Offer)
            {
                client.SetRemoteDescription(new RtcDescription()
                {
                    Sdp = host.LocalDescription,
                    Type = RtcDescriptionType.Offer
                });
            }
        };

        client.OnLocalDescriptionSafe += (_, desc) =>
        {
            _output.WriteLine("Client local description changed to type " + desc.Type);

            if (desc.Type == RtcDescriptionType.Answer)
            {
                host.SetRemoteDescription(new RtcDescription()
                {
                    Sdp = desc.Sdp,
                    Type = desc.Type
                });
            }
        };

        client.OnDataChannel += (_, channel) =>
        {
            _output.WriteLine($"Client: got channel {channel.Label}");

            channel.OnOpen += _ =>
            {
                _output.WriteLine($"Client: channel {channel.Label} open");
                channelOpenedEvent.Set();
            };

            channel.OnClose += _ =>
            {
                _output.WriteLine($"Client: channel {channel.Label} closed");
                channelClosedEvent.Set();
            };

            channel.OnBinaryReceivedSafe += (_, evt) =>
            {
                if (evt.Data.SequenceEqual(testBinary))
                    binaryReceivedEvent.Set();
            };
            
            channel.OnTextReceivedSafe += (_, evt) =>
            {
                if(evt.Text == textString)
                    textReceivedEvent.Set();
            };
        };
        var hostChannel1 = host.CreateDataChannel(new RtcCreateDataChannelArgs() { Label = "texttest" });

        hostChannel1.OnOpen += _ =>
        {
            _output.WriteLine("Sending binary data");
            hostChannel1.Send(testBinary);
            _output.WriteLine("Sending text data");
            hostChannel1.Send(textString);
        };

        if(!connectedEvent.Wait(TimeSpan.FromSeconds(5)))
            Assert.Fail("The peers did not connect in time");

        if(!channelOpenedEvent.Wait(TimeSpan.FromSeconds(5)))
            Assert.Fail("The client did not open the data channel in time");

        _output.WriteLine($"Host local description: {host.LocalDescription}");
        _output.WriteLine($"Host local description type: {host.LocalDescriptionType}");
        _output.WriteLine($"Host remote description: {host.RemoteDescription}");
        _output.WriteLine($"Host remote description type: {host.RemoteDescriptionType}");
        _output.WriteLine($"Host local address: {host.LocalAddress}");
        _output.WriteLine($"Host remote address: {host.RemoteAddress}");

        if(host.TryGetSelectedCandidatePair(out var pair))
            _output.WriteLine($"Selected candidates: {pair.LocalCandidate} | {pair.RemoteCandidate}");
        else
            Assert.Fail("Failed to get selected candidate pairs");

        _output.WriteLine($"Client local description: {client.LocalDescription}");
        _output.WriteLine($"Client local description type: {client.LocalDescriptionType}");
        _output.WriteLine($"Client remote description: {client.RemoteDescription}");
        _output.WriteLine($"Client remote description type: {client.RemoteDescriptionType}");
        _output.WriteLine($"Client local address: {client.LocalAddress}");
        _output.WriteLine($"Client remote address: {client.RemoteAddress}");
        
        _output.WriteLine("Peers connected, waiting for binary data");

        if(!binaryReceivedEvent.Wait(TimeSpan.FromSeconds(5)))
            Assert.Fail("The client did not receive binary data in time");
        
        _output.WriteLine("Client received binary data, waiting for text");
        
        if(!textReceivedEvent.Wait(TimeSpan.FromSeconds(5)))
            Assert.Fail("The client did not receive text data in time");
        
        _output.WriteLine("Client received text data. Closing peers");
        
        host.Dispose();
        client.Dispose();

        if(!channelClosedEvent.Wait(TimeSpan.FromSeconds(5)))
            Assert.Fail("The DataChannel close event was not raised");

        if (!closedEvent.Wait(TimeSpan.FromSeconds(5)))
            Assert.Fail("Client or host connection did not close in time");

        _output.WriteLine("Peers closed");
    }
}