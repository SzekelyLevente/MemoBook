namespace MemoBook

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

type EndPoint =
    | [<EndPoint "/" ; Method("GET","POST")>] Login
    | [<EndPoint "/main">] Main
    | [<EndPoint "/create">] Create
    | [<EndPoint "/settings">] Settings
    | [<EndPoint "/update">] Update

module Templating =
    open WebSharper.UI.Html

    // Compute a menubar where the menu item for the given endpoint is active
    let MenuBar (ctx: Context<EndPoint>) endpoint : Doc list =
        let ( => ) txt act =
            let isActive = if endpoint = act then "nav-link active" else "nav-link"
            li [attr.``class`` "nav-item"] [
                a [
                    attr.``class`` isActive
                    attr.href (ctx.Link act)
                ] [text txt]
            ]
        [
            "Main" => EndPoint.Main
            "Create" => EndPoint.Create
            "Settings" => EndPoint.Settings
            "Logout" => EndPoint.Login
        ]

    let Main ctx action (title: string) (body: Doc list) =
        Content.Page(
            Templates.MainTemplate()
                .Title(title)
                .MenuBar(MenuBar ctx action)
                .Body(body)
                .Doc()
        )

    let Login ctx action (title: string) (body: Doc list) =
        Content.Page(
            Templates.MainTemplate()
                .Title(title)
                .Body(body)
                .Doc()
        )

module Site =
    open WebSharper.UI.Html

    let getSessionSet (ctx: Context<EndPoint>)=
        let u = Var.Create ""
        async{
            let username=ctx.UserSession.GetLoggedInUser() |> Async.RunSynchronously
            match username with
            | Some usr -> u.Value <- usr
            | None -> u.Value <- ""
        }|>Async.StartImmediate
        u.Value <> ""

    let messageWindow header message=
        div [attr.``class`` "toast show fixed-bottom"] [
            div [attr.``class`` "toast-header"] [
                strong [attr.``class`` "me-auto"] [text header]
                
                button [attr.``type`` "button" ; attr.``class`` "btn-close" ; attr.``data-`` "bs-dismiss" "toast"] []
            ]
            div [attr.``class`` "toast-body"] [
                p [] [text message]
            ]
        ]

    let HomePage (ctx: Context<EndPoint>) (message:string) header=
        let isLogedIn=getSessionSet ctx
        if isLogedIn then
            let userid=Var.Create ""
            async {
                let! u=ctx.UserSession.GetLoggedInUser()
                userid.Value <- u.Value
            } |> Async.StartImmediate
            let user=Client.getUser(int(userid.Value))
            let welcomeText="Welcome "+user.name
            let registries=Client.getRegistryParams(System.DateTime.MinValue,user.id,"")
            let registriesDoc = Doc.Concat (registries |> List.map(fun item ->
                div [attr.``class`` "row"] [
                    div [attr.``class`` "col-sm-12 rounded mt-2 mb-2 pt-2 pb-2 bg-secondary"] [
                        h3 [] [text (item.date.ToString("yyyy-MM-dd"))]
                        p [] [text item.text]
                        form [attr.method "POST"] [
                            input [attr.``type`` "hidden" ; attr.value (item.id.ToString()) ; attr.name "id"] []
                            button [attr.``type`` "submit" ; attr.``class`` "btn btn-danger" ; attr.name "delReg"] [text "delete"]
                            a [attr.href ("/update?id="+item.id.ToString()) ; attr.``class`` "btn btn-success"] [text "update"]
                        ]
                    ]
                ]
            ))
            Templating.Main ctx EndPoint.Main "Main" [
                div [attr.``class`` "container"] [
                    div [attr.``class`` "row"] [
                        div [attr.``class`` "col-sm-12 text-center"] [
                            h1 [] [text welcomeText]
                        ]
                    ]
                    registriesDoc
                    if message <> "" then
                        messageWindow header message
                ]
            ]
        else
            Content.Forbidden

    let LoginPage (ctx: Context<EndPoint>) message header=
        async{
            do! ctx.UserSession.Logout()
        }|>Async.StartImmediate
        Templating.Login ctx EndPoint.Login "Login" [
            div [attr.``class`` "container"] [
                div [attr.``class`` "row"] [
                    div [attr.``class`` "col-sm-6"] [
                        h2 [] [text "Login"]
                        form [attr.method "POST"] [
                            label [] [text "username"]
                            input [attr.``type`` "text" ; attr.``class`` "form-control" ; attr.name "username"] []
                            label [] [text "password"]
                            input [attr.``type`` "password" ; attr.``class`` "form-control" ; attr.name "password"] []
                            button [attr.``type`` "submit" ; attr.``class`` "btn btn-success mt-2" ; attr.name "login"] [text "login"]
                        ]
                    ]
                    div [attr.``class`` "col-sm-6"] [
                        h2 [] [text "Registration"]
                        form [attr.method "POST"] [
                            label [] [text "username"]
                            input [attr.``type`` "text" ; attr.``class`` "form-control" ; attr.name "username"] []
                            label [] [text "password"]
                            input [attr.``type`` "password" ; attr.``class`` "form-control" ; attr.name "password"] []
                            label [] [text "password again"]
                            input [attr.``type`` "password" ; attr.``class`` "form-control" ; attr.name "passwordA"] []
                            button [attr.``type`` "submit" ; attr.``class`` "btn btn-success mt-2" ; attr.name "regist"] [text "registration"]
                        ]
                    ]
                ]
                if message <> "" then
                    messageWindow header message
            ]
        ]

    let UpdatePage message header ctx=
        let isLogedIn=getSessionSet ctx
        if isLogedIn then
            if ctx.Request.Get.Item "id" <> None then
                let id=ctx.Request.Get.Item "id"
                let reg=Client.getRegistry(int(id.Value))
                let userid=Var.Create ""
                async {
                    let! u=ctx.UserSession.GetLoggedInUser()
                    userid.Value <- u.Value
                } |> Async.StartImmediate
                let user=Client.getUser(int(userid.Value))
                if reg.id <> 0 && reg.ownerid = user.id then
                    Templating.Main ctx EndPoint.Update "Update" [
                        div [attr.``class`` "row"] [
                            div [attr.``class`` "col-sm-12 text-center"] [
                                h1 [] [text "Update registry"]
                                form [attr.method "POST"] [
                                    label [] [text "date"]
                                    input [attr.``type`` "date" ; attr.``class`` "form-control" ; attr.name "date" ; attr.value (reg.date.ToString("yyyy-MM-dd"))] []
                                    label [] [text "text"]
                                    textarea [attr.``class`` "form-control" ; attr.name "text"] [text reg.text]
                                    input [attr.``type`` "hidden" ; attr.value id.Value ; attr.name "id"] []
                                    button [attr.``type`` "submit" ; attr.``class`` "btn btn-success mt-2" ; attr.name "update"] [text "update"]
                                ]
                            ]
                        ]
                        if message <> "" then
                            messageWindow header message
                    ]
                else
                    Content.Forbidden
            else
                Content.Forbidden
        else
            Content.Forbidden

    let CreatePage message header ctx=
        let isLogedIn=getSessionSet ctx
        if isLogedIn then
            Templating.Main ctx EndPoint.Create "Create" [
                div [attr.``class`` "row"] [
                    div [attr.``class`` "col-sm-12 text-center"] [
                        h1 [] [text "Create registry"]
                        form [attr.method "POST"] [
                            label [] [text "date"]
                            input [attr.``type`` "date" ; attr.``class`` "form-control" ; attr.name "date"] []
                            label [] [text "text"]
                            textarea [attr.``class`` "form-control" ; attr.name "text"] []
                            button [attr.``type`` "submit" ; attr.``class`` "btn btn-success mt-2" ; attr.name "create"] [text "create"]
                        ]
                    ]
                ]
                if message <> "" then
                    messageWindow header message
            ]
        else
            Content.Forbidden

    let SettingsPage (message:string) header ctx=
        let isLoggedIn=getSessionSet ctx
        if isLoggedIn then
            let userid=Var.Create ""
            async {
                let! u=ctx.UserSession.GetLoggedInUser()
                userid.Value <- u.Value
            } |> Async.StartImmediate
            let user=Client.getUser(int(userid.Value))
            Templating.Main ctx EndPoint.Settings "Settings" [
                div [attr.``class`` "row"] [
                    div [attr.``class`` "col-sm-12 text-center"] [
                        h1 [] [text "User settings"]
                    ]
                ]
                div [attr.``class`` "row"] [
                    div [attr.``class`` "col-sm-12 bg-secondary rounded mt-2 mb-2 pt-2 pb-2"] [
                        h3 [] [text "User name"]
                        form [attr.method "POST"] [
                            label [] [text "name"]
                            input [attr.``type`` "text" ; attr.``class`` "form-control" ; attr.name "name" ; attr.value user.name] []
                            button [attr.``type`` "submit" ; attr.``class`` "btn btn-success mt-2" ; attr.name "updateName"] [text "update"]
                        ]
                    ]
                ]
                div [attr.``class`` "row"] [
                    div [attr.``class`` "col-sm-12 bg-secondary rounded mt-2 mb-2 pt-2 pb-2"] [
                        h3 [] [text "User password"]
                        form [attr.method "POST"] [
                            label [] [text "current password"]
                            input [attr.``type`` "password" ; attr.``class`` "form-control" ; attr.name "currentP"] []
                            label [] [text "new password"]
                            input [attr.``type`` "password" ; attr.``class`` "form-control" ; attr.name "newP"] []
                            label [] [text "new password again"]
                            input [attr.``type`` "password" ; attr.``class`` "form-control" ; attr.name "newPa"] []
                            button [attr.``type`` "submit" ; attr.``class`` "btn btn-success mt-2" ; attr.name "updatePassword"] [text "update"]
                        ]
                    ]
                ]
                div [attr.``class`` "row"] [
                    div [attr.``class`` "col-sm-12 bg-secondary rounded mt-2 mb-2 pt-2 pb-2"] [
                        h3 [] [text "User delete"]
                        form [attr.method "POST"] [
                            label [] [text "type \"delete\" to delete"]
                            input [attr.``type`` "text" ; attr.``class`` "form-control" ; attr.name "delText"] []
                            button [attr.``type`` "submit" ; attr.``class`` "btn btn-danger mt-2" ; attr.name "delete"] [text "delete"]
                        ]
                    ]
                ]
                if message <> "" then
                    messageWindow header message
            ]
        else
            Content.Forbidden

    let update (ctx: Context<EndPoint>) =
        if ctx.Request.Post.Item "update" <> None then
            let date=ctx.Request.Post.Item "date"
            let text=ctx.Request.Post.Item "text"
            if date.Value <> "" && text.Value <> "" then
                let id=ctx.Request.Post.Item "id"
                let currReg=Client.getRegistry(int(id.Value))
                let newReg={id=currReg.id;date=System.DateTime.Parse(date.Value);ownerid=currReg.ownerid;text=text.Value}
                Client.updateRegistry(newReg,int(id.Value)) |> ignore
                Content.RedirectPermanent EndPoint.Main
            else
                UpdatePage "Please give all data!" "Update" ctx
        else
            UpdatePage "" "" ctx

    let main (ctx: Context<EndPoint>) =
        if ctx.Request.Post.Item "delReg" <> None then
            let id=ctx.Request.Post.Item "id"
            Client.deleteRegistry(int(id.Value)) |> ignore
            HomePage ctx "Registry deleted!" "Delete"
        else
            HomePage ctx "" ""

    let settings (ctx: Context<EndPoint>) =
        let userid=Var.Create ""
        async {
            let! u=ctx.UserSession.GetLoggedInUser()
            userid.Value <- u.Value
        } |> Async.StartImmediate
        let user=Client.getUser(int(userid.Value))
        if ctx.Request.Post.Item "updateName" <> None then
            let newName=ctx.Request.Post.Item "name"
            if newName.Value <> "" then
                let newUser={id=user.id;name=newName.Value;password=user.password}
                Client.updateUser(newUser,user.id) |> ignore
                SettingsPage "Name updated!" "Update" ctx
            else
                SettingsPage "Please give all data!" "Update" ctx
        elif ctx.Request.Post.Item "updatePassword" <> None then
            let currentP=ctx.Request.Post.Item "currentP"
            let newP=ctx.Request.Post.Item "newP"
            let newPa=ctx.Request.Post.Item "newPa"
            if currentP.Value <> "" && newP.Value <> "" && newPa.Value <> "" then
                if currentP.Value = user.password then
                    if newP.Value = newPa.Value then
                        let newUser={id=user.id;name=user.name;password=newP.Value}
                        Client.updateUser(newUser,user.id) |> ignore
                        SettingsPage "Password updated!" "Update" ctx
                    else
                        SettingsPage "The new passwords doesn\'t match!" "Update" ctx
                else
                    SettingsPage "The current password is wrong!" "Update" ctx
            else
                SettingsPage "Please give all data!" "Update" ctx
        elif ctx.Request.Post.Item "delete" <> None then
            let delText=ctx.Request.Post.Item "delText"
            if delText.Value = "delete" then
                Client.deleteUser(user.id) |> ignore
                Content.RedirectPermanent EndPoint.Login
            else
                SettingsPage "The text is not \"delete\"!" "Delete" ctx
        else
            SettingsPage "" "" ctx

    let create (ctx: Context<EndPoint>) =
        if ctx.Request.Post.Item "create" <> None then
            let date=ctx.Request.Post.Item "date"
            let text=ctx.Request.Post.Item "text"
            let userid=Var.Create ""
            async{
                let! u=ctx.UserSession.GetLoggedInUser()
                userid.Value <- u.Value
            } |> Async.StartImmediate
            let user=Client.getUser(int(userid.Value))
            if date.Value <> "" && text.Value <> "" then
                let newRegistry={ id=0 ; date=System.DateTime.Parse(date.Value) ; ownerid=user.id ; text=text.Value }
                Client.createRegistry(newRegistry) |> ignore
                Content.RedirectPermanent EndPoint.Main
            else
                CreatePage "Please give all data!" "Create" ctx
        else
            CreatePage "" "" ctx

    let login (ctx: Context<EndPoint>) =
        if ctx.Request.Post.Item "login" <> None then
            let username=ctx.Request.Post.Item "username"
            let password=ctx.Request.Post.Item "password"
            if username.Value <> "" && password.Value <> "" then
                let user=Client.getUserParams(username.Value,password.Value)
                if user.Length <> 0 then
                    async{
                        do! ctx.UserSession.LoginUser(user.[0].id.ToString())
                    }|>Async.StartImmediate
                    Content.RedirectPermanent EndPoint.Main
                else
                    LoginPage ctx "Wrong username or password!" "Login"
            else
                LoginPage ctx "Please give all data!" "Login"
        elif ctx.Request.Post.Item "regist" <> None then
            let username=ctx.Request.Post.Item "username"
            let password=ctx.Request.Post.Item "password"
            let passwordA=ctx.Request.Post.Item "passwordA"
            if username.Value <> "" && password.Value <> "" && passwordA.Value <> "" then
                if password.Value = passwordA.Value then
                    let tryUser=Client.getUserParams(username.Value,"")
                    if tryUser.Length = 0 then
                        let user={ id=0 ; name=username.Value ; password=password.Value}
                        Client.createUser(user) |> ignore
                        LoginPage ctx "Account created!" "Create"
                    else
                        LoginPage ctx "This username is exists!" "Create"
                else
                    LoginPage ctx "The password\'s doesn\'t match!" "Create"
            else
                LoginPage ctx "Please give all data!" "Create"
        else
            LoginPage ctx "" ""

    [<Website>]
    let Main =
        Application.MultiPage (fun (ctx: Context<EndPoint>) endpoint ->
            match endpoint with
            | EndPoint.Main -> 
                match ctx.Request.Method with
                | Http.Get -> HomePage ctx "" ""
                | Http.Post -> main ctx
                | _ -> Content.Forbidden
            | EndPoint.Create -> 
                match ctx.Request.Method with
                | Http.Get -> CreatePage "" "" ctx
                | Http.Post -> create ctx
                | _ -> Content.NotFound
            | EndPoint.Login ->
                match ctx.Request.Method with
                | Http.Get -> LoginPage ctx "" ""
                | Http.Post -> login ctx
                | _ -> Content.NotFound
            | EndPoint.Settings ->
                match ctx.Request.Method with
                | Http.Get -> SettingsPage "" "" ctx
                | Http.Post -> settings ctx
                |_ -> Content.NotFound
            | EndPoint.Update ->
                match ctx.Request.Method with
                | Http.Get -> UpdatePage "" "" ctx
                | Http.Post -> update ctx
                | _ -> Content.NotFound
        )