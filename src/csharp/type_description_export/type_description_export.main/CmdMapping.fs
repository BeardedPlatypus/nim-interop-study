namespace type_description_export.main

open Elmish
open type_description_export.infrastructure

module public CmdMapping =
    open type_description_export.presentation.Main

    let private initializeCmd () : Cmd<Msg> =
        async {
            do! Async.SwitchToThreadPool ()
            return Msg.UpdateFiles ( SourceCode.retrieveFileNames() )
        } |> Cmd.OfAsync.result

    let private openVisualStudioCodeCmd () : Cmd<Msg> =
        async {
            do! Async.SwitchToThreadPool ()
            VisualStudioCode.start ()
            return Msg.NoOp
        } |> Cmd.OfAsync.result

    let private loadSourceContentCmd (v: CachedChangeFile) : Cmd<Msg> =
        async {
            do! Async.SwitchToThreadPool ()

            if v.retryCount > 5 then 
                return Msg.NoOp
            else
                try 
                    return v.fileName
                    |> Option.map (fun v -> SourceCode.readFile(v))
                    |> Msg.UpdateSourceContent
                with
                | :? System.IO.IOException ->
                    return Msg.ChangeFileCached v

        } |> Cmd.OfAsync.result

    let private setSelectedFileCmd (fileName: option<string>) : Cmd<Msg> =
        Cmd.ofMsg (Msg.SetSelectedFile fileName)

    let public toCmd (cmdMsg: CmdMsg) : Cmd<Msg> =
        match cmdMsg with
        | CmdMsg.Initialize -> initializeCmd ()  
        | CmdMsg.OpenVisualStudioCode -> openVisualStudioCodeCmd ()
        | CmdMsg.LoadSourceContent v -> loadSourceContentCmd v
        | CmdMsg.RequestSetSelectedFile v -> setSelectedFileCmd v
