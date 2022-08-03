namespace type_description_export.presentation

open Elmish.WPF

module public Main =
    type public Model = 
      { todo : bool
      }

    [<RequireQualifiedAccess>]
    type public CmdMsg =
        | Initialize

    type public Msg =
        | NoOp

    let public init (): Model * CmdMsg list = { todo = true }, [ CmdMsg.Initialize ]

    let public update (msg: Msg) (model: Model) : Model * CmdMsg list =
        match msg with 
        | NoOp -> model, []

    let bindings () : Binding<Model, Msg> list = []
