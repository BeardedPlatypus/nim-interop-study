import sample_struct
import calc

const nElements = 4

type
    xElementArray = array[nElements, cdouble]
    yElementArray = array[nElements, cdouble]

var x_elems: xElementArray = [ 1.0, 2.0, 3.0, 4.0 ]
var y_elems: yElementArray = [ 5.0, 6.0, 7.0, 8.0 ]

var s = SampleStruct(xElems: x_elems[0].unsafeAddr,
                     yElems: y_elems[0].unsafeAddr,
                     nNodes: nElements)

let v = custom_calculate(s)
echo v
