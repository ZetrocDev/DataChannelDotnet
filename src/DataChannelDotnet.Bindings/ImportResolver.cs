using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DataChannelDotnet.Bindings;
public static class ImportResolver
{
    private const string LibraryName = "datachannel";


#pragma warning disable CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
    [ModuleInitializer]
#pragma warning restore CA2255 // The 'ModuleInitializer' attribute should not be used in libraries

    internal static void Init()
    {
        NativeLibrary.SetDllImportResolver(typeof(ImportResolver).Assembly, DllImportResolver);
    }

    //This is here because we need to decide which native libraries to load, if the user has not specified
    //an RID.
    private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName == LibraryName)
        {
            var assemblyDir = Path.GetDirectoryName(assembly.Location) ?? AppContext.BaseDirectory;

            var rid = RuntimeInformation.RuntimeIdentifier;
            var fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "datachannel.dll" :
                          RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "libdatachannel.dylib" :
                          "libdatachannel.so";

            // runtimes/win-x64/native/datachannel.dll
            var nativeLibPath = Path.Combine(assemblyDir, "runtimes", rid, "native", fileName);

            if (File.Exists(nativeLibPath))
            {
                return NativeLibrary.Load(nativeLibPath);
            }

            var fallbackPath = Path.Combine(assemblyDir, fileName);
            if (File.Exists(fallbackPath))
            {
                return NativeLibrary.Load(fallbackPath);
            }
        }

        return IntPtr.Zero;
    }
}