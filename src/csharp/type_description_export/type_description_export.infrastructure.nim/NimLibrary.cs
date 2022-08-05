using System.Runtime.InteropServices;
using System.Text;

namespace type_description_export.infrastructure.nim;

public sealed class NimLibrary : IDisposable
{
    private const string _libraryPath = "./nim/type_description_export.dll";
    private Wrapper? _wrapper = null;
    
    private class Wrapper : IDisposable
    {
        private readonly IntPtr _pDLL;
        private readonly Delegates.NimMain _nimMain;
        private readonly Delegates.get_n_components _getNComponents;
        private readonly Delegates.get_component_name_length _getComponentNameLength;
        private readonly Delegates.get_component_name _getComponentName;
        private readonly Delegates.get_n_fields _getNFields;
        private readonly Delegates.get_field_name_length _getFieldNameLength;
        private readonly Delegates.get_field_name _getFieldName;
        private readonly Delegates.get_field_type_index _getFieldTypeIndex;
        private readonly Delegates.get_type_name_length _getTypeNameLength;
        private readonly Delegates.get_type_name _getTypeName;

        public Wrapper()
        {
            _pDLL = NativeLibrary.Load(_libraryPath);

            _nimMain = GetDelegate<Delegates.NimMain>(nameof(Delegates.NimMain));
            _getNComponents = GetDelegate<Delegates.get_n_components>(nameof(Delegates.get_n_components));
            _getComponentNameLength = GetDelegate<Delegates.get_component_name_length>(nameof(Delegates.get_component_name_length));
            _getComponentName = GetDelegate<Delegates.get_component_name>(nameof(Delegates.get_component_name));
            _getNFields = GetDelegate<Delegates.get_n_fields>(nameof(Delegates.get_n_fields));
            _getFieldNameLength = GetDelegate<Delegates.get_field_name_length>(nameof(Delegates.get_field_name_length));
            _getFieldName = GetDelegate<Delegates.get_field_name>(nameof(Delegates.get_field_name));
            _getFieldTypeIndex = GetDelegate<Delegates.get_field_type_index>(nameof(Delegates.get_field_type_index));
            _getTypeNameLength = GetDelegate<Delegates.get_type_name_length>(nameof(Delegates.get_type_name_length));
            _getTypeName = GetDelegate<Delegates.get_type_name>(nameof(Delegates.get_type_name));
        }

        private T GetDelegate<T>(string name) where T : Delegate
        {
            IntPtr address = NativeLibrary.GetExport(_pDLL, name);
            return Marshal.GetDelegateForFunctionPointer<T>(address);
        }

        public void NimMain() => _nimMain!.Invoke();
        public int GetNumberComponents() => _getNComponents();
        public int GetComponentNameLength(int indexComponent) => _getComponentNameLength(indexComponent);
        public void GetComponentName(int indexComponent, StringBuilder buffer) => _getComponentName(indexComponent, buffer);
        public int GetNumberFields(int indexComponent) => _getNFields(indexComponent);
        public int GetFieldNameLength(int indexComponent, int indexField) => _getFieldNameLength(indexComponent, indexField);
        public void GetFieldName(int indexComponent, int indexField, StringBuilder buffer) => _getFieldName(indexComponent, indexField, buffer);
        public int GetFieldTypeIndex(int indexComponent, int indexField) => _getFieldTypeIndex(indexComponent, indexField);
        public int GetTypeNameLength(int indexType) => _getTypeNameLength(indexType);
        public void GetTypeName(int indexType, StringBuilder buffer) => _getTypeName(indexType, buffer);

        private static class Delegates
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void NimMain();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int get_n_components();

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int get_component_name_length(int component_index);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void get_component_name(int component_index, StringBuilder buffer);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int get_n_fields(int component_index);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int get_field_name_length(int component_index, int field_index);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void get_field_name(int component_index, int field_name, StringBuilder buffer);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int get_field_type_index(int component_index, int field_name);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int get_type_name_length(int type_index);

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void get_type_name(int type_index, StringBuilder buffer);
        }

        private void ReleaseUnmanagedResources() => NativeLibrary.Free(_pDLL);

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Wrapper() => ReleaseUnmanagedResources();
    }

    public bool CanLoad => !HasLoaded && File.Exists(_libraryPath);
    public bool HasLoaded => _wrapper != null;

    public void Load()
    {
        if (!CanLoad) return;
        _wrapper = new Wrapper();
    }

    public void Unload()
    {
        if (!HasLoaded) return;

        var contextWeakRef = new WeakReference(_wrapper, trackResurrection: true);
        _wrapper!.Dispose();
        _wrapper = null;

        // Ensure the _context has been garbage collected
        // See: https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability#use-a-custom-collectible-assemblyloadcontext
        for (int i = 0; contextWeakRef.IsAlive && (i < 10); i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public void Initialize() => _wrapper!.NimMain();

    public IEnumerable<NimTypeDescription> GetComponents()
    {
        int nComponents = _wrapper!.GetNumberComponents();
        return Enumerable.Range(0, nComponents)
                         .Select(GetComponent);
    }

    private NimTypeDescription GetComponent(int componentIndex) =>
        new(GetComponentName(componentIndex),
            GetComponentFields(componentIndex));

    private string GetComponentName(int componentIndex)
    {
        int size = _wrapper!.GetComponentNameLength(componentIndex);
        StringBuilder buffer = new(size);
        _wrapper.GetComponentName(componentIndex, buffer);
        return buffer.ToString();
    }

    private IReadOnlyList<NimFieldDescription> GetComponentFields(int componentIndex)
    {
        NimFieldDescription GetComponentField(int fieldIndex) =>
            GetField(componentIndex, fieldIndex);

        int nFields = _wrapper!.GetNumberFields(componentIndex);
        return Enumerable.Range(0, nFields)
                         .Select(GetComponentField)
                         .ToList();
    }

    private NimFieldDescription GetField(int componentIndex, int fieldIndex) =>
        new(GetFieldName(componentIndex, fieldIndex),
            GetFieldType(componentIndex, fieldIndex));

    private string GetFieldName(int componentIndex, int fieldIndex)
    {
        int size = _wrapper!.GetFieldNameLength(componentIndex, fieldIndex);
        StringBuilder buffer = new(size);
        _wrapper.GetFieldName(componentIndex, fieldIndex, buffer);
        return buffer.ToString();
    }

    private string GetFieldType(int componentIndex, int fieldIndex)
    {
        int typeIndex = _wrapper!.GetFieldTypeIndex(componentIndex, fieldIndex);
        int size = _wrapper.GetTypeNameLength(typeIndex);
        StringBuilder buffer = new(size);
        _wrapper.GetTypeName(typeIndex, buffer);
        return buffer.ToString();
    }

    public void Dispose()
    {
        _wrapper?.Dispose();
    }
}