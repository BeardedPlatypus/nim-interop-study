open System
open Elmish.WPF

open type_description_export.presentation.views

[<EntryPoint; STAThread>]
let main _ =
    Program.mkProgramWpfWithCmdMsg
        type_description_export.presentation.Main.init 
        type_description_export.presentation.Main.update 
        type_description_export.presentation.Main.bindings
        type_description_export.main.CmdMapping.toCmd
    |> Program.runWindow (MainView())
