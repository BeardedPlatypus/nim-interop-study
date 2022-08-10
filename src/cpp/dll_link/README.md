# dll_link implements a very simple library with a C-api

The `Core` solution implements a very simple C++ project which exposes
a `calculate` method utilizing some simple struct. This method is 
exposed as part of the C-api of the build DLL:

```cpp
#pragma once
#define DLLAPI __declspec( dllexport )

#include "sample_struct.hpp"

namespace core {
  extern "C" DLLAPI double custom_calculate(sample_struct & s);
}
```

Note that the `extern "C"` is required to provide a proper C style function.
The function itself takes a mutable reference to a `sample_struct`. 
This `sample_struct` will need to be defined in the consuming library as well.