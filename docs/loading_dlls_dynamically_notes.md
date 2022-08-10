# Loading DLLs dynamically in C\# can be achieved with `NativeLibrary.Load` method

*An example of this code can be found in [`NimLibrary.cs`](/src/csharp/type_description_export/type_description_export.infrastructure.nim/NimLibrary.cs)*

In order to dynamically import functions from a DLL we need to use `NativeLibrary.Load` and subsequently reflection to retrieve the appropriate functions:

```csharp
IntPtr _pDLL = NativeLibrary.Load(LibraryPath)
```

It is important to keep track of the returned `IntPtr` if, for whatever reason, we need
to unload the DLL again. Which could be done with:

```csharp
        private void ReleaseUnmanagedResources() => NativeLibrary.Free(_pDLL);

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        // Assuming your function is called Wrapper
        ~Wrapper() => ReleaseUnmanagedResources();
```

Once the DLL has been loaded, it is recommended to create delegates out of the functions to import.
This should reduce the performance impact of retrieving the function each time they are called:

```csharp
private T GetDelegate<T>(string name) where T : Delegate
{
    IntPtr address = NativeLibrary.GetExport(_pDLL, name);
    return Marshal.GetDelegateForFunctionPointer<T>(address);
}
```

Here a delegate pointing to a function with `name` is retrieved from the DLL. This is done by first
retrieving the address and then utilizing the `Marshal` library to create a delegate pointer.
See the [`Marshal.GetDelegateForFunctionPointer` documentation](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.getdelegateforfunctionpointer?view=net-6.0) for more details.

The type should be a delegate, and could be specified as follows:

```csharp
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void NimMain();
```

Once a delegate has been created it could be exposed to the managed code as follows:

```csharp
public void NimMain() => _nimMain!.Invoke();
```

This approach has been based upon the code provided in [Jonathan Swift's Blog](https://docs.microsoft.com/en-us/archive/blogs/jonathanswift/dynamically-calling-an-unmanaged-dll-from-net-c).