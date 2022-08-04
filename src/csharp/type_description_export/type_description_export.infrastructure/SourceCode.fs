﻿namespace type_description_export.infrastructure

open System.IO
open System.Runtime.Caching

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

