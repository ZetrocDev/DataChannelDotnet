using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.Marshalling;
using DataChannelDotnet.Bindings;
using DataChannelDotnet.Data;
using DataChannelDotnet.Impl;

namespace DataChannelDotnet.Internal;

internal static unsafe class RtcHelpers
{
    public delegate int StringFetchFunc(int id, sbyte* buffer, int len);
    public static string GetString(int id, StringFetchFunc func, Lock @lock, ref bool disposed, string callerName)
    {
        using (@lock.EnterScope())
        {
            ObjectDisposedException.ThrowIf(disposed, callerName);

            return GetString(id, func);
        }
    }
    
    /// <summary>
    /// Tries to retrieve a string from an unmanaged function without allocating anything (Except the string).
    /// if it's too large, it rents a buffer instead.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static string GetString(int id, StringFetchFunc func)
    {
        const int maxStackallocSize = 4096;
        int len = func(id, null, 0).GetValueOrThrow();

        if (len == 0)
            return string.Empty;

        if (len > maxStackallocSize)
        {
            //Too large, just rent a buffer
            return GetStringRented(id, func);
        }
        else
        {
            //Small enough to stackalloc
            Span<byte> buffer = stackalloc byte[len];

            fixed (byte* bufferPtr = buffer)
            {
                int hr = func(id, (sbyte*)bufferPtr, buffer.Length);

                if (hr == -4) //-4 is the error code meaning 'buffer too small'. This could happen if the local description has changed since the previous call to 
                                //acquire the size
                    return GetStringRented(id, func);

                if (hr == 0)
                    return string.Empty;

                //If it's a different error code, then just throw
                hr.GetValueOrThrow();
                return Utf8StringMarshaller.ConvertToManaged(bufferPtr) ?? string.Empty;
            }
        }
    }

    private static string GetStringRented(int id, StringFetchFunc func)
    {
        using (IMemoryOwner<byte> buff = MemoryPool<byte>.Shared.Rent(1024*256))
        {
            fixed (byte* bufferPtr = buff.Memory.Span)
            {
                int len = func(id, (sbyte*)bufferPtr, buff.Memory.Span.Length).GetValueOrThrow();

                if (len == 0)
                    return string.Empty;
                
                return Utf8StringMarshaller.ConvertToManaged(bufferPtr) ?? string.Empty;
            }
        }
    }
    
    public static bool TryGetSelectedCandidatePair(int peerId, Lock @lock, ref bool disposed, [NotNullWhen(true)] out RtcCandidatePair? pair)
    {
        pair = default;

        using var _ = @lock.EnterScope();
        ObjectDisposedException.ThrowIf(disposed, nameof(RtcPeerConnection));

        using IMemoryOwner<byte> localCandidateBuffer = MemoryPool<byte>.Shared.Rent(1024 * 128);
        using IMemoryOwner<byte> remoteCandidateBuffer = MemoryPool<byte>.Shared.Rent(1024 * 128);

        fixed (byte* localCandidatePtr = localCandidateBuffer.Memory.Span)
            fixed (byte* remoteCandidatePtr = remoteCandidateBuffer.Memory.Span)
        {
            int maxLen = Rtc.rtcGetSelectedCandidatePair(peerId, (sbyte*)localCandidatePtr, localCandidateBuffer.Memory.Length,
                                (sbyte*)remoteCandidatePtr, remoteCandidateBuffer.Memory.Length);
            if (maxLen <= 0)
                return false;

            pair = new RtcCandidatePair()
            {
                LocalCandidate = Utf8StringMarshaller.ConvertToManaged(localCandidatePtr),
                RemoteCandidate = Utf8StringMarshaller.ConvertToManaged(remoteCandidatePtr)
            };

            return true;
        }
    }
    
    public static RtcDescriptionType ParseSdpType(sbyte* ptr)
    {
        Span<byte> typeBuffer = stackalloc byte[16];
        int len = Utf8Helper.GetUtf8Chars(ptr, typeBuffer);
        typeBuffer = typeBuffer.Slice(0, len);
        
        if (typeBuffer.SequenceEqual("answer"u8))
            return RtcDescriptionType.Answer;
        else if (typeBuffer.SequenceEqual("offer"u8))
            return RtcDescriptionType.Offer;
        else if (typeBuffer.SequenceEqual("pranswer"u8))
            return RtcDescriptionType.PrAnswer;
        else if (typeBuffer.SequenceEqual("rollback"u8))
            return RtcDescriptionType.Rollback;
        
        throw new InvalidOperationException("Unknown description type");
    }
}