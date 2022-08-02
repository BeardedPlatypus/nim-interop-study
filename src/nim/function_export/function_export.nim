# compile ensures the following c file is compiled as part of the dll.
{.compile: "function_export.c"}

# The pragma's `exportc` and `dynlib` ensure add is added to the capi of the dll.
proc add(a: float, b: float): float {. exportc, dynlib .} =
  a + b
