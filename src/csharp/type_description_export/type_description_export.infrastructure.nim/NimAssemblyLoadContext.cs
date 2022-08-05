using System.Reflection;
using System.Runtime.Loader;

namespace type_description_export.infrastructure.nim;

public class NimAssemblyLoadContext : AssemblyLoadContext
{
    public NimAssemblyLoadContext() : base(isCollectible: true) { }
    protected override Assembly? Load(AssemblyName name) => null;
}