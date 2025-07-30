using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DataChannelDotnet;
using DataChannelDotnet.Data;
using DataChannelDotnet.Events;
using DataChannelDotnet.Impl;

BenchmarkRunner.Run<Bench>();

[MemoryDiagnoser(displayGenColumns: true)]
public class Bench
{
    private IRtcPeerConnection _hostPeer = null!;
    private IRtcDataChannel _hostDataChannel = null!;

    private IRtcPeerConnection _clientPeer = null!;
    private IRtcDataChannel _clientDataChannel = null!;

    const int _maxMessageSize = 1000*256;
    private byte[] _bytes = new byte[1000*1000*256];
    private int _dataSize = 50*1000*1000;

    private ManualResetEventSlim _waitEvent = new(false);

    private int _bytesReceived = 0;

    [GlobalSetup]
    public void Setup()
    {
        new Random().NextBytes(_bytes);

        using ManualResetEventSlim _waitEvent = new ManualResetEventSlim(false);

        _hostPeer = new RtcPeerConnection(new RtcPeerConfiguration() { MaxMessageSize = _maxMessageSize });
        _clientPeer = new RtcPeerConnection(new RtcPeerConfiguration() { MaxMessageSize = _maxMessageSize });

        _hostPeer.OnCandidateSafe += (_, cnd) => _clientPeer.AddRemoteCandidate(cnd);
        _clientPeer.OnCandidateSafe += (_, cnd) => _hostPeer.AddRemoteCandidate(cnd);

        _hostPeer.OnLocalDescriptionSafe += (_, description) => _clientPeer.SetRemoteDescription(description);
        _clientPeer.OnLocalDescriptionSafe += (_, description) => _hostPeer.SetRemoteDescription(description);

        _clientPeer.OnDataChannel += (_, channel) =>
        {
            _clientDataChannel = channel;

            _clientDataChannel.OnBinaryReceivedSafe += OnClientData;

            _clientDataChannel.OnOpen += (_) =>
            {
                Console.WriteLine("Client data channel opened");
                _waitEvent.Set();
            };
        };

        _hostDataChannel = _hostPeer.CreateDataChannel(new RtcCreateDataChannelArgs { Label = "Test", Protocol = RtcDataChannelProtocol.Binary });
        _hostDataChannel.OnOpen += (_) =>
        {
            Console.WriteLine("Host data channel opened");
        };

        if (!_waitEvent.Wait(5000))
            throw new TimeoutException();
    }

    void OnClientData(IRtcDataChannel channel, RtcBinaryReceivedEventSafe e)
    {
        _bytesReceived += e.Data.Length;

        if (_bytesReceived >= _dataSize)
        {
            _waitEvent.Set();
        }
    }

    [Benchmark]
    public void SendAndReceive100MB()
    {
        _dataSize = 1000 * 1000 * 100; // 50 MB

        try
        {
            for (int i = 0; i < (_dataSize / _maxMessageSize) + 1; i++)
            {
                _hostDataChannel.Send(new Span<byte>(_bytes, 0, _maxMessageSize));
            }

            if (!_waitEvent.Wait(30000))
            {
                throw new TimeoutException("Data was not received in time.");
            }
        }
        finally
        {
            _waitEvent.Reset();
        }
    }

    [Benchmark]
    public void SendAndReceive50MB()
    {
        _dataSize = 1000*1000 * 50; // 50 MB

        try
        {
            for (int i = 0; i < (_dataSize/_maxMessageSize)+1; i++)
            {
                _hostDataChannel.Send(new Span<byte>(_bytes, 0, _maxMessageSize));
            }

            if(!_waitEvent.Wait(30000))
            {
                throw new TimeoutException("Data was not received in time.");
            }
        }
        finally
        {
            _waitEvent.Reset();
        }
    }

    [Benchmark]
    public void SendAndReceive1MB()
    {
        _dataSize = 1000 * 1000; // 1 MB

        try
        {
            for (int i = 0; i < (_dataSize / _maxMessageSize) + 1; i++)
            {
                _hostDataChannel.Send(new Span<byte>(_bytes, 0, _maxMessageSize));
            }

            if (!_waitEvent.Wait(30000))
            {
                throw new TimeoutException("Data was not received in time.");
            }
        }
        finally
        {
            _waitEvent.Reset();
        }
    }

    [Benchmark]
    public void SendAndReceive256KB()
    {
        _dataSize = 256*1000;

        try
        {
            for (int i = 0; i < (_dataSize / _maxMessageSize) + 1; i++)
            {
                _hostDataChannel.Send(new Span<byte>(_bytes, 0, _maxMessageSize));
            }

            if (!_waitEvent.Wait(30000))
            {
                throw new TimeoutException("Data was not received in time.");
            }
        }
        finally
        {
            _waitEvent.Reset();
        }
    }

    [IterationSetup]
    public void SetupIteration()
    {
        _bytesReceived = 0;
        _waitEvent.Reset();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _hostDataChannel?.Dispose();
        _hostPeer?.Dispose();
        _clientDataChannel?.Dispose();
        _clientPeer?.Dispose();

        _waitEvent.Dispose();
    }
}