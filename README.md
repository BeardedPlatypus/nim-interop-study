# Nim / C# / C++ interop study

This repository contains a simple study in the feasibility of using 
[Nim](https://nim-lang.org) within some of my personal projects. Within some
of my projects I want to embed a (simple) programming language, which can be 
used to extend / modify some of the behavior offered in the application. 
Nim is a prime candidate for this. This repository contains some (simplified)
behavior which will be used within some actual personal projects.

Note that I am an absolute beginner when it comes to Nim. As such, please do not
take the provided code samples here as gospel.

## Samples

This repository currently contains the following samples:

* `export_function`: A simple nim / C\# application capable illustrating the use a c-api.  
    - [nim folder](src/nim/function_export/README.md)
    - [C\# folder](src/csharp/function_export/README.md)