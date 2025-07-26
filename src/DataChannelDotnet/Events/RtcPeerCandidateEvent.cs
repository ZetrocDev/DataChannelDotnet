namespace DataChannelDotnet.Events;

public readonly ref struct RtcPeerCandidateEvent
{
    public readonly unsafe sbyte* Candidate;
    public readonly unsafe sbyte* Mid;

    internal unsafe RtcPeerCandidateEvent(sbyte* candidate, sbyte* mid)
    {
        Candidate = candidate;
        Mid = mid;
    }
}