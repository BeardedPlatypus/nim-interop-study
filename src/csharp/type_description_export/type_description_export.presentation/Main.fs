namespace type_description_export.presentation

open Elmish.WPF

module public Main =
    type public Model = 
      { files: List<string>
        selectedFile: option<string>
        sourceContent: option<string>
      }

    [<RequireQualifiedAccess>]
    type public CmdMsg =
        | Initialize
        | OpenVisualStudioCode
        | LoadSourceContent of option<string>

    type public Msg =
        | RequestOpenVisualStudioCode
        | UpdateFiles of List<string>
        // These could be combined
        | AddFile of string
        | RemoveFile of string
        | RenameFile of {| oldFile: string; newFile: string |}
        | SetSelectedFile of option<string>
        | NoOp
        | UpdateSourceContent of option<string>

    let public init (): Model * CmdMsg list = 
        { files = [ ]
          selectedFile = option.None
          sourceContent = option.None 
        }, [ CmdMsg.Initialize ]

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
            let newSelectedFile = if (Some toRemove) = model.selectedFile then None else model.selectedFile
            { model with files = newFiles; selectedFile = newSelectedFile }, []
        | RenameFile details ->
            let newFiles = details.newFile :: model.files |> List.filter (fun s -> s <> details.oldFile)
                           |> List.sort
            let newSelectedFile = if (Some details.oldFile) = model.selectedFile then None else model.selectedFile
            { model with files = newFiles; selectedFile = newSelectedFile }, []
        | SetSelectedFile v -> { model with selectedFile = v }, [CmdMsg.LoadSourceContent v]
        | UpdateSourceContent v -> {model with sourceContent = v }, []
        | NoOp -> model, []

    let bindings () : Binding<Model, Msg> list = [
          "OpenVisualStudioCommand" |> Binding.cmd(fun (_) -> Msg.RequestOpenVisualStudioCode)
          "FileNames" |> Binding.oneWay(fun (m: Model) -> m.files)
          "SelectedFile" |> Binding.twoWayOpt(
              (fun (m: Model) -> m.selectedFile),
              (fun v _ -> SetSelectedFile v)
          )
          "SourceContent" |> Binding.oneWay(fun (m: Model) -> m.sourceContent |> Option.defaultValue "<No file selected>")
    ]
