namespace type_description_export.presentation

open Elmish.WPF
open type_description_export.common.Records

module public Main =
    type public Model = 
      { files: List<string>
        selectedFile: option<string>
        sourceContent: option<string>
        types: List<ComponentDescription>
        selectedType: option<string>
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
        | AddFile of string
        | RemoveFile of string
        | RenameFile of {| oldFile: string; newFile: string |}
        | ChangeFile of string
        | ChangeFileCached of CachedChangeFile
        | SetSelectedFile of option<string>
        | SetSelectedType of option<string>
        | UpdateTypes of ComponentDescription list
        | NoOp
        | UpdateSourceContent of option<string>

    let public init (): Model * CmdMsg list = 
        { files = [ ]
          selectedFile = option.None
          sourceContent = option.None 
          types = []
          selectedType = option.None
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
        | UpdateTypes l -> { model with types = l}, []
        | NoOp -> model, []

    let retrieveSelectedFields (selected: string option) (m: Model): FieldDescription seq =
        let getFields (v: string) = 
            ( List.tryFind (fun e -> e.componentName = v) m.types )
            |> Option.map (fun v -> v.fields)
            |> Option.defaultWith (fun _ -> [])

        selected 
        |> (Option.map getFields) 
        |> (Option.defaultWith (fun _ -> []))
        |> Seq.ofList

    let bindings () : Binding<Model, Msg> list = [
          "OpenVisualStudioCommand" |> Binding.cmd (fun (_) -> Msg.RequestOpenVisualStudioCode)
          "FileNames" |> Binding.oneWay (fun (m: Model) -> m.files)
          "SelectedFile" |> Binding.twoWayOpt(
              (fun (m: Model) -> m.selectedFile),
              (fun v _ -> SetSelectedFile v)
          )
          "SourceContent" |> Binding.oneWay (fun (m: Model) -> m.sourceContent |> Option.defaultValue "<No file selected>")
          "Types" |> Binding.oneWay (fun (m: Model) -> m.types |> List.map (fun v -> v.componentName))
          "SelectedType" |> Binding.twoWayOpt(
              (fun (m: Model) -> m.selectedType),
              (fun v _ -> SetSelectedType v)
          )
          "TypeContent" |> (Binding.subModelSeq (
              (fun (m: Model) -> (retrieveSelectedFields m.selectedType m)), 
              (fun (e: FieldDescription) -> e.fieldName),
              (fun () -> [
                  "FieldName" |> Binding.oneWay (fun (_, m) -> m.fieldName)
                  "TypeName" |> Binding.oneWay (fun (_, m) -> m.typeName)
              ])))
          "CompileCommand" |> Binding.cmd (fun (_) -> Msg.RequestCompile)
    ]
