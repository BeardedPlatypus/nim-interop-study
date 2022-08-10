# The dll_link folder illustrates how to import c-style functions from an external dll

The dll_link folder contains several functions which utilize functions from the `Core.dll`
written in C++.

`sample_struct.nim` and `calc.nim` files have been produced with the `c2nim` tool:

```powershell
c2nim ../../cpp/dll_link/Core/Core/include/sample_struct.hpp> -o="sample_struct.nim" --nep1
c2nim ../../cpp/dll_link/Core/Core/include/calculate.hpp> -o="calc.nim" --nep1
```

The dll from which the `calculate` function is imported is specified with the `dynlib` pragma
value:

```nim
proc custom_calculate*(s: var SampleStruct): cdouble {. cdecl, dynlib: "Core.dll", importc .}
```
