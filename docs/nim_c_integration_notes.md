# Nim / C integration

## Importing a function in a .c file can be done with `importc` pragma

*See [memory_mapped_file](/src/nim/memory_mapped_file/) for an example.*

A function defined in a `.c` file can be forwarded with the following statements:

```nim
# Ensure the .c file is imported / linked correctly
{.compile: "mapping.c".}

# Define the call
proc CreateBufferFile(): pointer {.importc.}
```

If no string value is defined for `importc`, it will look for the exact same name as
the proc name. Otherwise you can explicitly state by specifying a specific function.
Furthermore, a header file can be specified with the `header` pragma.

For more details see [the importc pragma documentation](https://nim-lang.org/docs/manual.html#foreign-function-interface-importc-pragma).

## Importing a function from an external DLL

*See [dll_link](/src/nim/dll_link/) for an example*

A function defined in an external DLL can be forwarded with the following statement:

```nim
proc custom_calculate(s: var SampleStruct): cdouble {. cdecl, dynlib: "Core.dll", importc .}
```

The library is specified with the `dynlib` pragma. This approach does require the external
function to be exported as a c function (i.e. with `extern "C"`), and exported to the dll:

```cpp
#pragma once
#define DLLAPI __declspec( dllexport ) 

#include "sample_struct.hpp"

namespace core {
  extern "C" DLLAPI double custom_calculate(sample_struct & s);
}
```

[The c2nim tool](https://github.com/nim-lang/c2nim) can be used to automatically parse
header files to nim, to aid with wrapping an external API.

Calling the `c2nim` tool without any properties will display the help menu, explaining
the different parameters which can be specified.

## Exporting a function to a dll

*See [memory_mapped_file](/src/nim/memory_mapped_file/) for an example.*

A function defined in Nim can be exported to a DLL by adding the `exportc` and `dynlib`
pragma's to a proc:

```nim
proc Initialize*() {.exportc, dynlib.} =
    hMapFile = CreateBufferFile()
    pBuffer = CreateBuffer(hMapFile)
```

For more details see [the exportc pragma documentation](https://nim-lang.org/docs/manual.html#foreign-function-interface-exportc-pragma)