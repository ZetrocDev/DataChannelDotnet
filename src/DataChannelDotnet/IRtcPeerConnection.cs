using System.Diagnostics.CodeAnalysis;
using DataChannelDotnet.Bindings;
using DataChannelDotnet.Data;
using DataChannelDotnet.Events;

namespace DataChannelDotnet;

public interface IRtcPeerConnection : IDisposable
{
    event Action<IRtcPeerConnection, rtcState>? OnConnectionStateChange;
    event Action<IRtcPeerConnection, rtcGatheringState>? OnGatheringStateChange;
    event Action<IRtcPeerConnection, rtcSignalingState>? OnSignalingStateChange;
    event Action<IRtcPeerConnection, rtcIceState>?  OnIceStateChange;

    event Action<IRtcPeerConnection, IRtcDataChannel>? OnDataChannel;
    event Action<IRtcPeerConnection, IRtcTrack>? OnTrack;

    event Action<IRtcPeerConnection, RtcPeerCandidateEvent> OnCandidate;
    event Action<IRtcPeerConnection, RtcCandidate> OnCandidateSafe;

    event Action<IRtcPeerConnection, RtcLocalDescriptionEvent>? OnLocalDescription;
    event Action<IRtcPeerConnection, RtcDescription>? OnLocalDescriptionSafe;

    rtcState ConnectionState { get; }
    rtcIceState IceState { get; }
    rtcGatheringState GatheringState { get; }
    rtcSignalingState SignalingState { get; }
    bool NegotiationRequired { get; }
    string? LocalDescription { get; }
    RtcDescriptionType LocalDescriptionType { get; }
    string? RemoteDescription { get; }
    RtcDescriptionType RemoteDescriptionType { get; }
    string LocalAddress { get; }
    string RemoteAddress { get; }

    IRtcDataChannel CreateDataChannel(RtcCreateDataChannelArgs args);
    IRtcTrack CreateTrack(RtcCreateTrackArgs args);

    void AddRemoteCandidate(RtcCandidate candidate);

    void SetRemoteDescription(RtcDescription description);
    void SetLocalDescription(RtcDescriptionType type);

    bool TryGetSelectedCandidatePair([NotNullWhen(true)] out RtcCandidatePair? pair);
}