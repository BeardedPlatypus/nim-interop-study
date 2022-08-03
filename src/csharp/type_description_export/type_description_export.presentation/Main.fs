namespace type_description_export.presentation

open Elmish.WPF

module public Main =
    type public Model = 
      { files: List<string>
      }

    [<RequireQualifiedAccess>]
    type public CmdMsg =
        | Initialize
        | OpenVisualStudioCode

    type public Msg =
        | RequestOpenVisualStudioCode
        | UpdateFiles of List<string>
        | NoOp

    let public init (): Model * CmdMsg list = { files = [ ] }, [ CmdMsg.Initialize ]

    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | RequestOpenVisualStudioCode -> model, [CmdMsg.OpenVisualStudioCode]
        | UpdateFiles newFiles -> { model with files = newFiles }, []
        | NoOp -> model, []

    let bindings () : Binding<Model, Msg> list = [
          "OpenVisualStudioCommand" |> Binding.cmd(fun (_) -> Msg.RequestOpenVisualStudioCode)
          "FileNames" |> Binding.oneWay(fun (m: Model) -> m.files)
    ]
