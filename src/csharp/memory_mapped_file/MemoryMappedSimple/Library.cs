using System.Runtime.InteropServices;
using System.Text;

namespace memory_mapped_file;

public static class Library
{
    private const string Name = "memory_mapping_core";

    [DllImport(Name)]
    public static extern void NimMain();

    [DllImport(Name)]
    public static extern void Initialize();

    [DllImport(Name)]
    public static extern void Free();

    [DllImport(Name)]
    public static extern void Write(int v);

    [DllImport(Name)]
    public static extern int Read();
}