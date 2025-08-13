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

    //This is here because we need to decide which native libraries to load if the user has not specified
    //an RID. If this RID is set, the native libraries will be copied to the root of the build directory, otherwise
    //we need to figure out which file to load.

    //It seems like ubuntu (at least of the github actions runner) uses an ubuntu specific RID instead of linux-x64.
    private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName == LibraryName)
        {
            var assemblyDir = Path.GetDirectoryName(assembly.Location) ?? AppContext.BaseDirectory;

            string? fileName = null;
            string? rid = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName = "datachannel.dll";
                rid = Environment.Is64BitProcess ? "win-x64" : "win-x86";
            }else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fileName = "datachannel.so";
                rid = "linux-x64";
            }else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                fileName = "datachannel.dylib";
                rid = "osx-x64";
            }

            if (rid is null || fileName is null)
                return IntPtr.Zero;

            string runtimeSpecificPath = Path.Combine(assemblyDir, "runtimes", rid, "native", fileName);

            if (File.Exists(runtimeSpecificPath))
                return NativeLibrary.Load(runtimeSpecificPath);

            string fallbackPath = Path.Combine(assemblyDir, fileName);

            if (File.Exists(fallbackPath))
                return NativeLibrary.Load(fallbackPath);

        }

        return IntPtr.Zero;
    }
}