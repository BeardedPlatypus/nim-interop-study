# `memory_mapped_file` shows a simple implementation of shared memory in Nim

`memory_mapping_core.nim` exposes a simple implementation of using not-persisted memory
mapped files in Nim/C which can subsequently be opened in C\#. This logic can be used
to implement Inter-process communication (IPC).

The not-persisted memory mapped file creation is implemented in `mapping.c`. A buffer
is created with the name `Local\NimBuffer` through the methods `CreateBufferFile` and
`CreateBuffer`. The memory can be freed with `FreeBuffer` and `FreeBufferFile`.
Unfortunately, implementing not-persisted memory mapped files is not well supported in
`std/memfiles`, as such these calls are implemented directly in C and imported in the
`memory_mapping_core.nim` file.

Data can be written and read with the `moveMem` function.