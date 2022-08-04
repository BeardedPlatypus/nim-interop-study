namespace type_description_export.infrastructure

open System.IO

module public SourceCode =
    let public retrieveFileNames () : List<string> =
        let dirInfo = DirectoryInfo("./nim/custom-types/")
        
        dirInfo.EnumerateFiles()
        |> Seq.filter (fun fi -> fi.Extension = ".nim")
        |> Seq.map (fun fi -> fi.Name)
        |> Seq.toList

