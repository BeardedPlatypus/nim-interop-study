using System.Runtime.InteropServices;
using System.Text;

namespace type_description_export.infrastructure.nim.core;

public static class TypeDescriptionExport
{
    private static IntPtr _libraryHandle;
    public static void Initialize() => 
        _libraryHandle = NativeLibrary.Load(NameDLL);

    public static void Free() =>
        NativeLibrary.Free(_libraryHandle);

    private const string NameDLL = "./nim/type_description_export.dll";

    [DllImport(NameDLL)]
    public static extern void NimMain();

    [DllImport(NameDLL)]
    public static extern int get_n_components();

    [DllImport(NameDLL)]
    public static extern int get_component_name_length(int component_index);

    [DllImport(NameDLL)]
    public static extern void get_component_name(int component_index, StringBuilder buffer);

    [DllImport(NameDLL)]
    public static extern int get_n_fields(int component_index);

    [DllImport(NameDLL)]
    public static extern int get_field_name_length(int component_index, int field_index);

    [DllImport(NameDLL)]
    public static extern void get_field_name(int component_index, int field_name, StringBuilder buffer);

    [DllImport(NameDLL)]
    public static extern int get_field_type_index(int component_index, int field_name);

    [DllImport(NameDLL)]
    public static extern int get_type_name_length(int type_index);

    [DllImport(NameDLL)]
    public static extern void get_type_name(int type_index, StringBuilder buffer);
}