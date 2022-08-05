namespace type_description_export.infrastructure

open System.Diagnostics
open System.IO
open System.Runtime.Caching

open type_description_export.infrastructure.nim
open type_description_export.common.Records

module public SourceCode =
    let public path : string = "./nim/custom-types/"

    let public retrieveFileNames () : List<string> =
        let dirInfo = DirectoryInfo(path)
        
        dirInfo.EnumerateFiles()
        |> Seq.filter (fun fi -> fi.Extension = ".nim")
        |> Seq.map (fun fi -> fi.Name)
        |> Seq.toList

    let public readFile (fileName: string) : string = 
        Path.Combine(path, fileName)
        |> File.ReadAllLines 
        |> String.concat "\n"

    let mutable notifyRemovedMemoryCache = 
        fun (evArgs: CacheEntryRemovedArguments) -> ()

    let public cacheItem (key: string) (item: 'a) : unit =
        let policy = CacheItemPolicy()
        policy.RemovedCallback <- notifyRemovedMemoryCache
        policy.AbsoluteExpiration <- System.DateTimeOffset.Now.AddSeconds(0.5)
        MemoryCache.Default.AddOrGetExisting(key, item, policy) |> ignore

    let public writeCustomTypesFile (path: string) (typeFiles: list<string>): unit =
        typeFiles 
        |> List.map Path.GetFileNameWithoutExtension
        |> List.map (fun name -> $"import custom-types/{name}")
        |> fun lines -> File.WriteAllLines(path, lines)

    let public compile (): unit =
        let processStartInfo = ProcessStartInfo("nim", @"c --app:lib -d:release --outdir:./nim ./nim/type_description_export.nim")
        processStartInfo.UseShellExecute <- false
        processStartInfo.RedirectStandardOutput <- true

        let p = Process.Start(processStartInfo)
        p.WaitForExit()

    let private library : NimLibrary = new NimLibrary();

    let public loadLibrary = library.Load
    let public unloadLibrary = library.Unload
    let public initialize = library.Initialize
    let public getComponents () = 
        let toField (inputDesc: NimFieldDescription) : FieldDescription =
            { fieldName = inputDesc.Name; typeName = inputDesc.FieldType }
        let toComponent (inputDesc: NimTypeDescription): ComponentDescription =
            { componentName = inputDesc.Name
              fields = inputDesc.Fields |> Seq.map toField |> Seq.toList
            }

        library.GetComponents ()
        |> Seq.map toComponent
        |> Seq.toList


