namespace MemoBook

open WebSharper
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Templating
open WebSharper.UI.Notation
open WebSharper.UI.Client
open Newtonsoft.Json

[<JavaScript>]
module Templates =

    type MainTemplate = Templating.Template<"Main.html", ClientLoad.FromDocument, ServerLoad.WhenChanged>

[<JavaScript>]
module Client =

    let About () =
        let num=Var.Create ""
        div [] [
            Doc.InputType.Text [] num
            p [] [text (System.String(Array.rev(num.View.V.ToCharArray())))]
        ]

    let getUsers()=
        let res=Var.Create []
        async{
            let! u=Server.getUsers()
            res.Value <- u
        }|>Async.StartImmediate
        res.Value

    let getRegistries()=
        let res=Var.Create []
        async{
            let! u=Server.getRegistries()
            res.Value <- u
        }|>Async.StartImmediate
        res.Value


    let getUser(id:int)=
        let emptyUser = { id = 0; name = ""; password = "" }
        let res=Var.Create emptyUser
        async{
            let! u=Server.getUser(id)
            res.Value <- u
        }|>Async.StartImmediate
        res.Value

    let getRegistry(id:int)=
        let emptyReg = { id = 0; date = System.DateTime.Now; ownerid = 0 ; text = "" }
        let res=Var.Create emptyReg
        async{
            let! u=Server.getRegistry(id)
            res.Value <- u
        }|>Async.StartImmediate
        res.Value

    let getUserParams(name:string,password:string)=
        let res=Var.Create []
        async{
            let! u=Server.getUserParams(name,password)
            res.Value <- u
        }|>Async.StartImmediate
        res.Value

    let getRegistryParams(date:System.DateTime,ownerid:int,text:string)=
        let res=Var.Create []
        async{
            let! u=Server.getRegistryParams(date,ownerid,text)
            res.Value <- u
        }|>Async.StartImmediate
        res.Value

    let createUser(user:User)=
        let emptyCreate = { Message = "" ; Id = 0 }
        let res=Var.Create emptyCreate
        async{
            let! u=Server.createUser(user)
            res.Value <- u
        }|>Async.StartImmediate
        res.Value

    let createRegistry(registry:Registry)=
        let emptyCreate = { Message = "" ; Id = 0 }
        let res=Var.Create emptyCreate
        async{
            let! u=Server.createRegistry(registry)
            res.Value <- u
        }|>Async.StartImmediate
        res.Value

    let updateUser(user:User,id:int)=
        let emptyUpdate = { Message = "" ; Rows = 0 }
        let res=Var.Create emptyUpdate
        async{
            let! u=Server.updateUser(user,id)
            res.Value <- u
        }|>Async.StartImmediate
        res.Value

    let updateRegistry(registry:Registry,id:int)=
        let emptyUpdate = { Message = "" ; Rows = 0 }
        let res=Var.Create emptyUpdate
        async{
            let! u=Server.updateRegistry(registry,id)
            res.Value <- u
        }|>Async.StartImmediate
        res.Value

    let deleteUser(id:int)=
        let emptyDelete = { Message = "" ; Rows = 0 }
        let res=Var.Create emptyDelete
        async{
            let! u=Server.deleteUser(id)
            res.Value <- u
        }|>Async.StartImmediate
        res.Value

    let deleteRegistry(id:int)=
        let emptyDelete = { Message = "" ; Rows = 0 }
        let res=Var.Create emptyDelete
        async{
            let! u=Server.deleteRegistry(id)
            res.Value <- u
        }|>Async.StartImmediate
        res.Value