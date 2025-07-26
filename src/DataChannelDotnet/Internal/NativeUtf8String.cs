using System.Runtime.InteropServices.Marshalling;

namespace DataChannelDotnet.Internal;

internal unsafe ref struct NativeUtf8String : IDisposable
{
    public sbyte* Ptr => _ptr;
    
    private sbyte* _ptr;
    
    public NativeUtf8String(string? str)
    {
        if (str is null)
            return;

        _ptr = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(str);
    }

    public void Dispose()
    {
        if(_ptr is null)
            return;

        Utf8StringMarshaller.Free((byte*)_ptr);
        _ptr = null;
    }
}