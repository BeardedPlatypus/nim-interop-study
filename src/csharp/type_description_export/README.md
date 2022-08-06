<p align='center'><img align='center' src='https://github.com/BeardedPlatypus/media-storage/blob/main/nim-interop-study/export_type.png?raw=true' width='50%'></p>

# `type_description_export` implements a GUI to compile and inspect Nim files.

The `type_description_export` implements a GUI with [Elmish.WPF](https://github.com/elmish/Elmish.WPF). 
The GUI monitors nim files created in a dedicated folder, and displays their content.
It allows the user to compile the nim files with a Nim compiler located in the PATH of
the user, and extract the descriptions of the compiled types.

## Building and running the application

The application can be build with Visual Studio 2022. Upon building it will create an 
executable at:

```
<repo-path>/src/csharp/type_description_export/type_description_export.main/bin/<configuration>/net6.0-windows/win10-x64/type_description_export.main.exe
```

In order for the application to run correctly, the users needs to have the following configured:

* Have Visual Studio Code installed, and the binary directory in the PATH variable.
* Have the Nim compiler installed, and the binary directory in the PATH variable
* Copy the files located at `src/nim/type_description_export` to `<bin-folder>/nim`
* Create an empty directory at `<bin-folder>/nim/custom-types`

Where the `<bin-folder>` is the folder in which `type_description_export.main.exe` is located.

Once this is done, the application should run.
