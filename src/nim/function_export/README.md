# `function_export` exposes some c-api calls as a dll

The different files within the `function_export` folder expose the following functions with the created dll:

* `add`: A simple addition method adding two doubles and returning the result, written 
    in Nim. ([src](function_export.nim))
* `cadd`: A simple addition method adding two doubles and returning the result, written
    in C. ([src](function_export.c))
* `get_name_size`: Get the size of the name, written in C. ([src](function_export.c))
* `get_name`: Get a name, written in C. ([src](function_export.c))

## `function_export.dll` is build with the nim compiler

The `function_export.dll` can be build with the nim compiler. The compiler can be 
downloaded [here](https://nim-lang.org/install.html). The installation instructions
on this page should be followed. 

Once the compiler is downloaded and configured, the dll can be build with the following
command:

```powershell
nim c --app:lib -d:release "<path-to-repository>\src\nim\function_export\function_export.nim"
```

Where `<path-to-repository>` should be replaced with the absolute path to your checkout
directory. Once run, the compiler should have produced a `function_export.dll` which
can be used by other programs.

## `function_export.dll` is used by function_export.main

The created `function_export.dll` is used by the [`function_export.main` C\# project](/src/csharp/function_export/README.md). 
The `dll` will need to be copied by hand to the `bin` folder of the project. See [the `README.md` of the C\# project](/src/csharp/function_export/README.md) for more details.