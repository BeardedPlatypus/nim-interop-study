namespace type_description_export.presentation

open Elmish.WPF

module public Main =
    type public Model = 
      { files: List<string>
        selectedFile: option<string>
        sourceContent: option<string>
        types: List<string>
        selectedType: option<string>
        typeContent: option<string>
      }

    type public CachedChangeFile = 
      { fileName: option<string>
        retryCount: int
      }

    [<RequireQualifiedAccess>]
    type public CmdMsg =
        | Initialize
        | OpenVisualStudioCode
        | LoadSourceContent of CachedChangeFile
        | RequestSetSelectedFile of option<string>
        | Compile of List<string>

    type public Msg =
        | RequestOpenVisualStudioCode
        | RequestCompile
        | UpdateFiles of List<string>
        // These could be combined
        | AddFile of string
        | RemoveFile of string
        | RenameFile of {| oldFile: string; newFile: string |}
        | ChangeFile of string
        | ChangeFileCached of CachedChangeFile
        | SetSelectedFile of option<string>
        | SetSelectedType of option<string>
        | NoOp
        | UpdateSourceContent of option<string>

    let public init (): Model * CmdMsg list = 
        { files = [ ]
          selectedFile = option.None
          sourceContent = option.None 
          types = []
          selectedType = option.None
          typeContent = option.None
        }, [ CmdMsg.Initialize ]

    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | RequestOpenVisualStudioCode -> model, [CmdMsg.OpenVisualStudioCode]
        | RequestCompile -> model, [CmdMsg.Compile model.files]
        | UpdateFiles newFiles -> { model with files = newFiles }, []
        | AddFile newFile -> 
            // Assumption: this will not be an expensive operation given the size of files.
            let newFiles = newFile :: model.files |> List.sort
            { model with files = newFiles }, []
        | RemoveFile toRemove ->
            let newFiles = model.files |> List.filter (fun s -> s <> toRemove)
            let cmdMsgs = 
                if model.selectedFile |> Option.contains toRemove then 
                    [ CmdMsg.RequestSetSelectedFile None ] 
                else 
                    []

            { model with files = newFiles }, cmdMsgs
        | RenameFile details ->
            let newFiles = details.newFile :: model.files |> List.filter (fun s -> s <> details.oldFile)
                           |> List.sort
            let cmdMsgs = 
                if model.selectedFile |> Option.contains details.oldFile then 
                    [ CmdMsg.RequestSetSelectedFile (Some details.newFile) ] 
                else 
                    []

            { model with files = newFiles }, cmdMsgs
        | ChangeFile file ->
            let cmdMsgs = 
                if model.selectedFile |> Option.contains file then 
                    [ CmdMsg.LoadSourceContent { fileName = model.selectedFile; retryCount = 0 } ] 
                else 
                    []
            model, cmdMsgs
        | ChangeFileCached v ->
            model, [ CmdMsg.LoadSourceContent { v with retryCount = v.retryCount + 1 } ]
        | SetSelectedFile v -> 
            { model with selectedFile = v }, 
            [ CmdMsg.LoadSourceContent { fileName = v; retryCount = 0 } ]
        | SetSelectedType v -> 
            { model with selectedType = v }, [ ]
        | UpdateSourceContent v -> {model with sourceContent = v }, []
        | NoOp -> model, []

    let bindings () : Binding<Model, Msg> list = [
          "OpenVisualStudioCommand" |> Binding.cmd (fun (_) -> Msg.RequestOpenVisualStudioCode)
          "FileNames" |> Binding.oneWay (fun (m: Model) -> m.files)
          "SelectedFile" |> Binding.twoWayOpt(
              (fun (m: Model) -> m.selectedFile),
              (fun v _ -> SetSelectedFile v)
          )
          "SourceContent" |> Binding.oneWay (fun (m: Model) -> m.sourceContent |> Option.defaultValue "<No file selected>")
          "Types" |> Binding.oneWay (fun (m: Model) -> m.types)
          "SelectedType" |> Binding.twoWayOpt(
              (fun (m: Model) -> m.selectedType),
              (fun v _ -> SetSelectedType v)
          )
          "TypeContent" |> Binding.oneWay (fun (m: Model) -> m.typeContent |> Option.defaultValue "<no type selected>")
          "CompileCommand" |> Binding.cmd (fun (_) -> Msg.RequestCompile)
    ]
