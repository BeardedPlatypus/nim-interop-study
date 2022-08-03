namespace type_description_export.presentation

open Elmish.WPF

module public Main =
    type public Model = 
      { todo : bool
      }

    [<RequireQualifiedAccess>]
    type public CmdMsg =
        | Initialize
        | OpenVisualStudioCode

    type public Msg =
        | RequestOpenVisualStudioCode
        | NoOp

    let public init (): Model * CmdMsg list = { todo = true }, [ CmdMsg.Initialize ]

    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | RequestOpenVisualStudioCode -> model, [CmdMsg.OpenVisualStudioCode]
        | NoOp -> model, []

    let bindings () : Binding<Model, Msg> list = [
          "OpenVisualStudioCommand" |> Binding.cmd(fun (_) -> Msg.RequestOpenVisualStudioCode)
    ]
