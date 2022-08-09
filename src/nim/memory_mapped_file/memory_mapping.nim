{.compile: "mapping.c".}
import std/memfiles
import std/streams
import winlean
import os

# let bufSize: DWORD = 256
# let mmapName: string = "testMemory"
# let msg: cstring = "Message from first process."

# TODO: Figure out mapFlags, add MAP_ANON and MAP_PRIVATE
# let memoryMappedFile: MemFile = memfiles.open(mmapName, mode = fmReadWrite, newFileSize=sizeof(int));
# let memoryMappedFileStream: MemMapFileStream = newMemMapFileStream(mmapName, mode=fmReadWrite, fileSize=sizeof(int32))
# let val: int32 = 9
# writeData(memoryMappedFileStream, val.unsafeAddr, sizeof(int32))
# var memoryMappedFileStream : MemMapFileStream

# let handle = createFileMappingW(INVALID_HANDLE_VALUE, nil, PAGE_READWRITE, 0, bufSize, mmapName)
# mapViewOfFileEx(handle, FILE_MAP_WRITE, 0, 0, 256)

# proc write_val*(val: int32) {.exportc, dynlib .} =
#     writeData(memoryMappedFileStream, val.unsafeAddr, sizeof(int32))

# proc initialise_library*() {.exportc, dynlib .} =
#     memoryMappedFileStream = newMemMapFileStream(mmapName, mode=fmReadWrite, fileSize=sizeof(int32))
#     GC_ref(memoryMappedFileStream)

# proc free_library*() {.exportc, dynlib .} =
#     GC_unref(memoryMappedFileStream)

# proc get_mmap_name_length*(): int {. exportc, dynlib .} =
#     len(mmapName) + 1

# proc get_mmap_name*(buffer: cstring): void {. exportc, dynlib .} =
#     let size = get_mmap_name_length()
#     moveMem(buffer[0].unsafeAddr, mmapName[0].unsafeAddr, size)

# proc get_mmap_size*(): int {. exportc, dynlib .} =
#     sizeof(int32)

# when (isMainModule):
#    initialise()
#    setPosition(memoryMappedFileStream, 0)
#    echo peekInt32(memoryMappedFileStream)

# proc newEIO(msg: string): ref IOError =
#  new(result)
#  result.msg = msg

proc CreateBufferFile() {.importc.}
proc CreateBuffer(hMapFile: pointer): pointer {.importc.}
proc FreeBuffer(hMapFile: pointer): void {.importc.}

# NOTE: This currently support windows only
# proc openNotPersisted*(filename: string, 
#                       mode: FileMode = fmRead,
#                       mappedSize = -1, 
#                       offset = 0, 
#                       newFileSize = -1,
#                       allowRemap = false, 
#                       mapFlags = cint(-1)): MemFile =
  ## opens a memory mapped file. If this fails, `OSError` is raised.
  ##
  ## `newFileSize` can only be set if the file does not exist and is opened
  ## with write access (e.g., with fmReadWrite).
  ##
  ##`mappedSize` and `offset`
  ## can be used to map only a slice of the file.
  ##
  ## `offset` must be multiples of the PAGE SIZE of your OS
  ## (usually 4K or 8K but is unique to your OS)
  ##
  ## `allowRemap` only needs to be true if you want to call `mapMem` on
  ## the resulting MemFile; else file handles are not kept open.
  ##
  ## `mapFlags` allows callers to override default choices for memory mapping
  ## flags with a bitwise mask of a variety of likely platform-specific flags
  ## which may be ignored or even cause `open` to fail if misspecified.
  ##
  ## Example:
  ##
  ## .. code-block:: nim
  ##   var
  ##     mm, mm_full, mm_half: MemFile
  ##
  ##   mm = memfiles.open("/tmp/test.mmap", mode = fmWrite, newFileSize = 1024)    # Create a new file
  ##   mm.close()
  ##
  ##   # Read the whole file, would fail if newFileSize was set
  ##   mm_full = memfiles.open("/tmp/test.mmap", mode = fmReadWrite, mappedSize = -1)
  ##
  ##   # Read the first 512 bytes
  ##   mm_half = memfiles.open("/tmp/test.mmap", mode = fmReadWrite, mappedSize = 512)

  # The file can be resized only when write mode is used:

#  if mode == fmAppend:
#    raise newEIO("The append mode is not supported.")
# 
#  var readonly = mode == fmRead
# 
#  when defined(windows):
#    
#    let cFileName: cstring = filename
#    echo repr(cFileName)
# 
#    result.fHandle = INVALID_HANDLE_VALUE
# 
#    let v = ['G','l','o','b','a','l', '\\', 't', 'e', 's', 't', '\0']
#    echo ""
#    echo repr(v)
# 
#    echo ""
#    let pBuffer = CreateBuffer()
#    echo repr(pBuffer)
#    result.mapHandle = pBuffer
#    
#    # result.mapHandle = createFileMappingW(
#    #   INVALID_HANDLE_VALUE,                             # Use paging file
#    #   nil,                                              # Default security
#    #   if readonly: PAGE_READONLY else: PAGE_READWRITE,  # Read and write access definition
#    #   0, 
#    #   cast[int32](newFileSize), 
#    #   v[0].unsafeAddr)
#    #  result.mapHandle = createFileMappingW(
#    #     INVALID_HANDLE_VALUE, 
#    #     nil,
#    #     PAGE_READONLY,
#    #     0,
#    #     0,
#    #     cFileName.unsafeAddr
#    # )
# 
#    echo result.mapHandle
# 
#    result.mem = mapViewOfFileEx(
#      result.mapHandle,
#      if readonly: FILE_MAP_READ else: FILE_MAP_WRITE,
#      0,
#      0,
#      cast[WinSizeT](newFileSize),
#      nil)
# 
#    result.size = newFileSize
# 
#    result.wasOpened = true
#  else:
#    raise newEIO("Linux is not supported.")

# let mmapName: string = "Global\\memory_mapped_test"
# let mmapName: string = "testMemory"
# var memoryMappedFile: MemFile

# proc read_val*(): int32 {.exportc, dynlib.} =
#    var v: int32 = 0
#    moveMem(v.unsafeAddr, memoryMappedFile.mem, sizeof(int32))
#    return v

# proc write_val*(val: int32) {. exportc, dynlib .} =
    # writeData(memoryMappedFile, val.unsafeAddr, sizeof(int32))
#    moveMem(memoryMappedFile.mem, val.unsafeAddr, sizeof(int32))

proc initialise_library*() {.exportc, dynlib .} =
    # memoryMappedFile = openNotPersisted(mmapName, fmReadWrite, newFileSize=sizeof(int32))
    # memoryMappedFile = memfiles.open("memoryTestData", mode=fmReadWriteExisting)
    CreateBufferFile()
    # let p2: pointer = CreateBuffer(p1)
    # echo "test" & repr(p1) # & " | " & repr(p2)

# proc get_mmap_name_length*(): int {. exportc, dynlib .} =
#    len(mmapName) + 1

# proc get_mmap_name*(buffer: cstring): void {. exportc, dynlib .} =
#    let size = get_mmap_name_length()
#    moveMem(buffer[0].unsafeAddr, mmapName[0].unsafeAddr, size)

# proc get_mmap_size*(): int {. exportc, dynlib .} =
#    sizeof(int32)

when (isMainModule):
   echo "Initialising..."
   initialise_library()
   var name: string = readLine(stdin)
   # echo "Writing 9 to memory mapped file..."
   # write_val(9)
   # echo "Reading value fro memory mapped file: " & $(read_val())
