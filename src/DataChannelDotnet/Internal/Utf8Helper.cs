namespace DataChannelDotnet.Internal;

internal static unsafe class Utf8Helper
{
    public static int GetUtf8Chars(sbyte* str, Span<byte> buffer)
    {
        if (str is null)
            return 0;
        
        //Leave room to add \0 if the buffer is not long enough
        for (int i = 0; i < buffer.Length-1; i++)
        {
            buffer[i] = (byte)str[i];
            
            if(str[i] == '\0')
                return i;
        }

        buffer[buffer.Length - 1] = 0;
        return buffer.Length - 1;
    }
}