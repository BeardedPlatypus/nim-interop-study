namespace type_description_export.infrastructure

open System.Diagnostics

module public VisualStudioCode =
    let public start () : unit =
        let processStartInfo = ProcessStartInfo("code", "folder ./nim/custom-types/")
        processStartInfo.UseShellExecute <- true
        Process.Start(processStartInfo) |> ignore

