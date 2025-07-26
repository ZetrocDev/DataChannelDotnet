using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DataChannelDotnet.Internal;

internal static class RtcThread
{
    [ThreadStatic]
    public static bool IsRtcThread;

    public static void SetRtcThread()
    {
        if (IsRtcThread)
            return;
        
        Thread.CurrentThread.Name = "RtcThread";
        IsRtcThread = true;
    }
    
    public static unsafe bool TryGetRtcObjectInstance<T>(void* userPtr, [NotNullWhen(true)] out T? instance) 
        where T : class
    {
        SetRtcThread();
        
        instance = GCHandle.FromIntPtr((nint)userPtr).Target as T; 
        return instance is not null;
    }
}