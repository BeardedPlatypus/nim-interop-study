﻿namespace type_description_export.presentation

open Elmish.WPF

module public Main =
    type public Model = 
      { files: List<string>
        sourceContent: option<string>
      }

    [<RequireQualifiedAccess>]
    type public CmdMsg =
        | Initialize
        | OpenVisualStudioCode

    type public Msg =
        | RequestOpenVisualStudioCode
        | UpdateFiles of List<string>
        // These could be combined
        | AddFile of string
        | RemoveFile of string
        | RenameFile of {| oldFile: string; newFile: string |}
        | NoOp

    let public init (): Model * CmdMsg list = { files = [ ]; sourceContent = option.None }, [ CmdMsg.Initialize ]

    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | RequestOpenVisualStudioCode -> model, [CmdMsg.OpenVisualStudioCode]
        | UpdateFiles newFiles -> { model with files = newFiles }, []
        | AddFile newFile -> 
            // Assumption: this will not be an expensive operation given the size of files.
            let newFiles = newFile :: model.files |> List.sort
            { model with files = newFiles }, []
        | RemoveFile toRemove ->
            let newFiles = model.files |> List.filter (fun s -> s <> toRemove)
            { model with files = newFiles }, []
        | RenameFile details ->
            let newFiles = details.newFile :: model.files |> List.filter (fun s -> s <> details.oldFile)
                           |> List.sort
            { model with files = newFiles }, []
        | NoOp -> model, []

    let bindings () : Binding<Model, Msg> list = [
          "OpenVisualStudioCommand" |> Binding.cmd(fun (_) -> Msg.RequestOpenVisualStudioCode)
          "FileNames" |> Binding.oneWay(fun (m: Model) -> m.files)
          "SourceContent" |> Binding.oneWay(fun (m: Model) -> m.sourceContent |> Option.defaultValue "<No file selected>")
    ]
