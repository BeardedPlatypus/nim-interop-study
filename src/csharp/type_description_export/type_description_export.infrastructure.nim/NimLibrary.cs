using System.Reflection;
using System.Text;

namespace type_description_export.infrastructure.nim;

public sealed class NimLibrary : IDisposable
{
    private readonly string _libraryPath;

    private NimAssemblyLoadContext? _context = null;
    private Assembly? _assembly = null;
    private TypeDescriptionExportWrapper? _wrapper = null;
    private bool _hasDisposed = false;

    public NimLibrary()
    {
        _libraryPath = Path.Combine(Directory.GetCurrentDirectory(), 
                                    "nim", 
                                    "type_description_export.infrastructure.nim.core.dll");
    }

    private class TypeDescriptionExportWrapper
    {
        private readonly Action _nimMain;
        private readonly Func<int> _getNComponents;
        private readonly Func<int, int> _getComponentNameLength;
        private readonly Action<int, StringBuilder> _getComponentName;
        private readonly Func<int, int> _getNFields;
        private readonly Func<int, int, int> _getFieldNameLength;
        private readonly Action<int, int, StringBuilder> _getFieldName;
        private readonly Func<int, int, int> _getFieldTypeIndex;
        private readonly Func<int, int> _getTypeNameLength;
        private readonly Action<int, StringBuilder> _getTypeName;

        public TypeDescriptionExportWrapper(Type wrapperType)
        {
            _nimMain = CreateDelegate<Action>(wrapperType, "NimMain");
            _getNComponents = CreateDelegate<Func<int>>(wrapperType, "get_n_components");
            _getComponentNameLength = CreateDelegate<Func<int, int>>(wrapperType, "get_component_name_length");
            _getComponentName = CreateDelegate<Action<int, StringBuilder>>(wrapperType, "get_component_name");
            _getNFields = CreateDelegate<Func<int, int>>(wrapperType, "get_n_fields");
            _getFieldNameLength = CreateDelegate<Func<int, int, int>>(wrapperType, "get_field_name_length");
            _getFieldName = CreateDelegate<Action<int, int, StringBuilder>>(wrapperType, "get_field_name");
            _getFieldTypeIndex = CreateDelegate<Func<int, int, int>>(wrapperType, "get_field_type_index");
            _getTypeNameLength = CreateDelegate<Func<int, int>>(wrapperType, "get_type_name_length");
            _getTypeName = CreateDelegate<Action<int, StringBuilder>>(wrapperType, "get_type_name");
        }

        private static T CreateDelegate<T>(Type wrapperType, string methodName) where T : Delegate =>
            wrapperType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)?
                       .CreateDelegate<T>(null)
                ?? throw new InvalidOperationException(); 

        public void NimMain() => 
            _nimMain.Invoke();

        public int GetNumberComponents() => 
            _getNComponents.Invoke();

        public int GetComponentNameLength(int componentIndex) => 
            _getComponentNameLength.Invoke(componentIndex);

        public void GetComponentName(int componentIndex, StringBuilder buffer) =>
            _getComponentName(componentIndex, buffer);

        public int GetNumberFields(int componentIndex) =>
            _getNFields.Invoke(componentIndex);

        public int GetFieldNameLength(int componentIndex, int fieldIndex) =>
            _getFieldNameLength.Invoke(componentIndex, fieldIndex);

        public void GetFieldName(int componentIndex, int fieldIndex, StringBuilder buffer) =>
            _getFieldName.Invoke(componentIndex, fieldIndex, buffer);

        public int GetFieldTypeIndex(int componentIndex, int fieldIndex) =>
            _getFieldTypeIndex(componentIndex, fieldIndex);

        public int GetTypeNameLength(int typeIndex) =>
            _getTypeNameLength(typeIndex);

        public void GetTypeName(int typeIndex, StringBuilder buffer) =>
            _getTypeName(typeIndex, buffer);
    }

    public bool CanLoad => !HasLoaded && File.Exists(_libraryPath);
    public bool HasLoaded => _context != null;

    public void Load()
    {
        if (!CanLoad) return;

        _context = new NimAssemblyLoadContext();
        _assembly = _context.LoadFromAssemblyPath(_libraryPath);
    }

    public void Unload()
    {
        if (!HasLoaded) return;

        var contextWeakRef = new WeakReference(_context, trackResurrection: true);
        _assembly = null;
        _wrapper = null;

        _context?.Unload();
        _context = null;

        // Ensure the _context has been garbage collected
        // See: https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability#use-a-custom-collectible-assemblyloadcontext
        for (int i = 0; contextWeakRef.IsAlive && (i < 10); i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public void Initialize()
    {
        // Assumption: Initialize is only called when the NimLibrary is loaded.
        var typeDescriptionExportType = _assembly?.GetType("type_description_export.infrastructure.nim.core.TypeDescriptionExport") ?? throw new InvalidOperationException();
        _wrapper = new TypeDescriptionExportWrapper(typeDescriptionExportType);
        _wrapper.NimMain();
    }

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
        if (!_hasDisposed)
        {
            Unload();
            _hasDisposed = true;
        }

        GC.SuppressFinalize(this);
    }
}