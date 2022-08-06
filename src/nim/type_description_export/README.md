# `type_description_export` provides some macro's to expose type descriptions through a c-api

The [`compile_macros.nim`](compile_macros.nim) provides two methods to automatically generate
type descriptions of specified types and expose them through a c-api:

* `exportComponent`: Marks a type to be exposed to the c-api
* `constructComponentDescriptions`: Creates the c-api logic

This code is used within the [GUI](/src/csharp/type_description_export/README.md) to generate 
type descriptions while running the GUI. The GUI generates a `custom_types.nim` file, which
in turn is used by [`type_description_export.nim`](type_description_export.nim) to create a
DLL which is loaded by the application.

The files within this folder are expected to be placed in a `nim` folder at the root of the
executable folder. The custom types are placed in the `nim/custom-types` folder.