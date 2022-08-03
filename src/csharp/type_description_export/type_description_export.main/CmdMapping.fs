namespace type_description_export.main

open Elmish

module public CmdMapping =
    open type_description_export.presentation.Main

    let private initializeCmd () : Cmd<Msg> =
        async {
            do! Async.SwitchToThreadPool ()
            return Msg.NoOp
        } |> Cmd.OfAsync.result

    let public toCmd (cmdMsg: CmdMsg) : Cmd<Msg> =
        match cmdMsg with
        | CmdMsg.Initialize -> initializeCmd ()  
