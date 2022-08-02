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
