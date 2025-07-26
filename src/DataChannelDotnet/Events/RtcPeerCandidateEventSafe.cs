namespace DataChannelDotnet.Events;

public sealed class RtcPeerCandidateEventSafe
{
    public string? Candidate { get;  }
    public string? Mid { get; }
    
    internal RtcPeerCandidateEventSafe(string? candidate, string? mid)
    {
        Candidate = candidate;
        Mid = mid;
    }
}