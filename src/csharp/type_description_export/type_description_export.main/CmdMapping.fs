namespace type_description_export.main

open Elmish
open type_description_export.infrastructure

module public CmdMapping =
    open type_description_export.presentation.Main

    let private initializeCmd () : Cmd<Msg> =
        async {
            do! Async.SwitchToThreadPool ()
            return Msg.NoOp
        } |> Cmd.OfAsync.result

    let private openVisualStudioCodeCmd () : Cmd<Msg> =
        async {
            do! Async.SwitchToThreadPool ()
            VisualStudioCode.start ()
            return Msg.NoOp
        } |> Cmd.OfAsync.result

    let public toCmd (cmdMsg: CmdMsg) : Cmd<Msg> =
        match cmdMsg with
        | CmdMsg.Initialize -> initializeCmd ()  
        | CmdMsg.OpenVisualStudioCode -> openVisualStudioCodeCmd ()
