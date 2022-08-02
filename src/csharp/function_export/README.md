# `function_export.main` uses the `function_export.dll` in a simple command line utility

The `function_export.dll` created out of [the `function_export` nim source](/src/nim/function_export/README.md) is utilized by [the `function_export.main/Program.cs`](function_export.main/Program.cs) to run some of the functions provided in this `dll`.
The calls are wrapped with a p/invoke interface located in [NimSource.cs](function_export.main/NimSource.cs), which in turn is called in the [Program.cs](function_export.main/Program.cs).

Note that after building the application, the generated `function_export.dll` will 
need to be copied by hand to the created bin folder.