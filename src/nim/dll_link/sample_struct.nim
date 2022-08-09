type
  SampleStruct* {.bycopy.} = object
    xElems*: ptr cdouble
    yElems*: ptr cdouble
    nNodes*: cint

