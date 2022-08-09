using System.IO.MemoryMappedFiles;
using memory_mapped_file;

Library.NimMain();
Library.Initialize();

var mmf = MemoryMappedFile.OpenExisting("Local\\NimBuffer", MemoryMappedFileRights.ReadWriteExecute, HandleInheritability.Inheritable);
using var accessor = mmf.CreateViewAccessor(0, sizeof(int));

Library.Write(13);

accessor.Read(0, out Int32 v);
Console.WriteLine($"Nim says: {v}");

Library.Free();
