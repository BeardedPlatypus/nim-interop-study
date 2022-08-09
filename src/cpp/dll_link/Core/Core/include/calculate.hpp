#pragma once
#define DLLAPI __declspec( dllexport ) //Export when building DLL

#include "sample_struct.hpp"

namespace core {
  extern "C" DLLAPI double custom_calculate(sample_struct & s);
}
