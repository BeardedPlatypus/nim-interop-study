{.compile: "mapping.c".}

proc CreateBufferFile(): pointer {.importc.}
proc CreateBuffer(hMapFile: pointer): pointer {.importc.}
proc FreeBuffer(pBuffer: pointer) {.importc.}
proc FreeBufferFile(hMapFile: pointer) {.importc.}

var hMapFile : pointer = nil
var pBuffer : pointer = nil

proc Initialize*() {.exportc, dynlib.} =
    hMapFile = CreateBufferFile()
    echo repr(hMapFile)

    pBuffer = CreateBuffer(hMapFile)

proc Free*() {.exportc, dynlib.} =
    FreeBuffer(pBuffer)
    FreeBufferFile(hMapFile)

proc Write*(v: int) {.exportc, dynlib.} =
    let c = v
    moveMem(pBuffer, c.unsafeAddr, sizeof(int))

proc Read*(): int {.exportc, dynlib.} =
    moveMem(result.unsafeAddr, pBuffer, sizeof(int))


# when (isMainModule):
#    Initialize()
#    echo repr(hMapFile)
#    Write(20)
#    echo Read()
#    Free()