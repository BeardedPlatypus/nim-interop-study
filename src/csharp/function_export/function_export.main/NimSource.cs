using System.Runtime.InteropServices;
using System.Text;

namespace function_export.main;

public static class NimSource
{
    private const string NameDLL = "function_export";

    [DllImport(NameDLL)]
    public static extern void NimMain();

    [DllImport(NameDLL)]
    public static extern double add(double x, double y);

    [DllImport(NameDLL)]
    public static extern double cadd(double x, double y);

    [DllImport(NameDLL)]
    public static extern int get_name_size();

    [DllImport(NameDLL)]
    public static extern void get_name(StringBuilder buffer);
}
