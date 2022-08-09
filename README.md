# Nim / C# / C++ interop study

This repository contains a simple study in the feasibility of using 
[Nim](https://nim-lang.org) within some of my personal projects. Within some
of my projects I want to embed a (simple) programming language, which can be 
used to extend / modify some of the behavior offered in the application. 
Nim is a prime candidate for this. This repository contains some (simplified)
behavior which will be used within some actual personal projects.

Note that I am an absolute beginner when it comes to Nim. As such, please do not
take the provided code samples here as gospel.

## Gallery

<p align='center'><img align='center' src='https://github.com/BeardedPlatypus/media-storage/blob/main/nim-interop-study/export_type.png?raw=true' width='50%'></p>

*A GUI displaying NIM source code and the types retrieved through a generated C-api*

## Samples

This repository currently contains the following samples:

* `export_function`: A simple nim / C\# application capable illustrating the use a c-api.  
    - [nim folder](src/nim/function_export/README.md)
    - [C\# folder](src/csharp/function_export/README.md)
* `type_description_export`: A simple GUI capable of showing nim-code and visualizing the compiled descriptions.
    - [nim folder](src/nim/type_description_export/README.md)
    - [F\# folder](src/csharp/type_description_export/README.md)
* `memory_mapped_files`: Some simple memory mapped file examples.
    - [nim folder](src/nim/memory_mapped_file/README.md)
    - [C\# folder](src/csharp/memory_mapped_file/README.md)
* `external_dll_link`: An example linking a Nim compilation to some external DLL made in C++.
    - [nim folder](src/nim/dll_link/README.md)
    - [C++ folder](src/cpp/dll_link/README.md)
