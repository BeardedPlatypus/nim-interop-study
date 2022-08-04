namespace type_description_export.infrastructure

open System.IO

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

