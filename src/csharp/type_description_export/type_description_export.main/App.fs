open System
open System.IO
open Elmish
open Elmish.WPF

open type_description_export.infrastructure
open type_description_export.presentation
open type_description_export.presentation.views


let toFileWatchSubscription(watcher: FileSystemWatcher) =
        

    let addFileSub dispatch =
        let notifyAddFile (evArgs: FileSystemEventArgs) =
            dispatch (Main.Msg.AddFile evArgs.Name)
        watcher.Created.Add(notifyAddFile)
    let removeFileSub dispatch =
        let notifyRemoveFile (evArgs: FileSystemEventArgs) =
            dispatch (Main.Msg.RemoveFile evArgs.Name)
        watcher.Deleted.Add(notifyRemoveFile)
    let renameFileSub dispatch = 
        let notifyRenameFile (evArgs: RenamedEventArgs) =
            dispatch (Main.Msg.RenameFile {| oldFile= evArgs.OldName; newFile= evArgs.Name |})
        watcher.Renamed.Add(notifyRenameFile)
    
    Cmd.batch [
        Cmd.ofSub addFileSub
        Cmd.ofSub removeFileSub
        Cmd.ofSub renameFileSub
    ]

[<EntryPoint; STAThread>]
let main _ =
    use watcher: FileSystemWatcher = new FileSystemWatcher(SourceCode.path)
    watcher.NotifyFilter <- ( NotifyFilters.Attributes 
                            ||| NotifyFilters.CreationTime
                            ||| NotifyFilters.DirectoryName
                            ||| NotifyFilters.FileName
                            ||| NotifyFilters.LastAccess
                            ||| NotifyFilters.LastWrite
                            ||| NotifyFilters.Security
                            ||| NotifyFilters.Size
                            )

    watcher.Filter <- "*.nim"
    watcher.EnableRaisingEvents <- true;

    Program.mkProgramWpfWithCmdMsg
        type_description_export.presentation.Main.init 
        type_description_export.presentation.Main.update 
        type_description_export.presentation.Main.bindings
        type_description_export.main.CmdMapping.toCmd
    |> Program.withSubscription (fun _ -> toFileWatchSubscription watcher)
    |> Program.runWindow (MainView())
