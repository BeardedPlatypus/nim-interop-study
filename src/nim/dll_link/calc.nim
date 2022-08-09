import sample_struct

proc custom_calculate*(s: var SampleStruct): cdouble {. cdecl, dynlib: "Core.dll", importc .}