using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace DataChannelDotnet.Internal;

internal unsafe ref struct NativeUtf8StringArray : IDisposable
{
    public sbyte** Ptr => _ptr;
    public int StringCount { get; }
    
    private sbyte** _ptr;

    public NativeUtf8StringArray(string[]? array)
    {
        if (array is null)
            return;
        
        StringCount = array.Length;
        _ptr = (sbyte**)NativeMemory.AllocZeroed((nuint)(StringCount * sizeof(nint)));

        try
        {
            for (int i = 0; i < StringCount; i++)
            {
                _ptr[i] = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(array[i]);
            }
        }
        catch (Exception)
        {
            Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (_ptr is null)
            return;

        for (var i = 0; i < StringCount; i++)
        {
            Utf8StringMarshaller.Free((byte*)_ptr[i]);
        }
        
        NativeMemory.Free(_ptr);
        _ptr = null;
    }
}