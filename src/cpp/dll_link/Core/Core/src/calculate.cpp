#include "calculate.hpp"

namespace core {
  double custom_calculate(sample_struct& s)
  {
    double result = 0.0;

    for (size_t i = 0; i < s.n_nodes; i++) {
      result += s.x_elems[i] * s.y_elems[i];
    }

    return result;
  }
}