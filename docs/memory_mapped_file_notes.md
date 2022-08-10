# Not-persisted memory-mapped files can be used to share memory between processes and dlls

In order to share memory between two different processes or unrelated DLLs is to make use of a
memory-mapped file. This process basically creates a block of memory which can be accessed by
other DLLs. This block of memory can be backed by a file on disk (persisted) or created purely
in memory (not-persisted). Within these notes we will only be concerned with not-persisted 
memory-mapped files.

## Accessing a memory-mapped file in C++ / Nim

Memory-mapped files in Nim can be created with the [std/memfiles](https://nim-lang.org/docs/memfiles.html)
library. Unfortunately this library does not seem to support Not-persisted memory-mapped files
out of the box. Trying to implement the code directly in Nim on top of `winlean` turned out to
be fruitless as well. Instead the code has been written directly in C++.

The not-persisted memory-mapped file can be created with:

```cpp
HANDLE hMapFile = CreateFileMapping(
    INVALID_HANDLE_VALUE,
    NULL,
    PAGE_READWRITE,
    0,
    BUFFER_SIZE,
    BUFFER_NAME);
```

Where `BUFFER_SIZE` is a number of bytes to create, and `BUFFER_NAME` a `TCHAR` array specifying
the name of the created memory-mapped file.

Once a file has been created, a view on this file can be constructed with:

```cpp
LPVOID p = MapViewOfFileEx(hMapFile, FILE_MAP_ALL_ACCESS, 0, 0, BUFFER_SIZE, NULL);
```

The returned pointer can be subsequently used to read and write data to the file.

In order to free the view and file the following code can be used:

```cpp
UnmapViewOfFile(pBuffer);
CloseHandle(hMapFile);
```

This will ensure everything is properly cleaned up.

The before mentioned calls can be wrapped in Nim:

```nim
{.compile: "mapping.c".}

proc CreateBufferFile(): pointer {.importc.}
proc CreateBuffer(hMapFile: pointer): pointer {.importc.}
proc FreeBuffer(pBuffer: pointer) {.importc.}
proc FreeBufferFile(hMapFile: pointer) {.importc.}
```

We can then define the appropriate read and write functions with `moveMem`:

```nim
proc Write*(v: int) {.exportc, dynlib.} =
    let c = v
    moveMem(pBuffer, c.unsafeAddr, sizeof(int))

proc Read*(): int {.exportc, dynlib.} =
    moveMem(result.unsafeAddr, pBuffer, sizeof(int))
```

## Accessing a memory-mapped file in C\#

In order to access the not-persisted memory-mapped files in C\# we can make use of 
`System.IO.MemoryMappedFiles`. Among others this library exposes the following functions:

* `MemoryMappedFile.CreateOrOpen`: Opens an existing memory-mapped file with the name or creates a new 
    Memory-mapped file with the specified size.
* `MemoryMappedFile.OpenExisting`: Opens an existing memory-mapped file with the specified name,
    if none exist an error is thrown

Once a `MemoryMappedFile` has been created, a view can be created with:

```csharp
using var accessor = mmf.CreateViewAccessor(0, BUFFER_SIZE);
```

This accessor can subsequently be used to read or write (depending on the provided arguments)
data from the Memory-mapped file.

## Implementation notes

Accessing the `Global` cache requires the process creating the Memory-mapped file to have elevated 
security permissions (i.e. it should run in Admin). This will make this memory block available to
other unrelated processes.

## References

The following msdn articles have been used to develop the code:

* [Memory-mapped files (C\#)](https://docs.microsoft.com/en-us/dotnet/standard/io/memory-mapped-files) 
* [Creating Named Shared Memory (C++)](https://docs.microsoft.com/en-us/windows/win32/memory/creating-named-shared-memory)