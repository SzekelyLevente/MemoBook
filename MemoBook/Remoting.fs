namespace MemoBook

open WebSharper
open System.Net.Http
open Newtonsoft.Json
open System

type User = {
    id: int
    name: string
    password: string
}

type Registry = {
    id: int
    date:System.DateTime
    ownerid:int
    text:string
}

type Create = {
    Message: string
    Id: int
}

type Update = {
    Message: string
    Rows: int
}

module Server =

    let apiKey = "Zama1975"
    let apiUrl = "https://allbreak.eu/"

    let rec queryValue key value =
        match box value with
        | :? string as s when s <> "" -> Some $"{key}={System.Net.WebUtility.UrlEncode(s)}"
        | :? int as i when i <> 0 -> Some $"{key}={i}"
        | :? bool as b when b -> Some $"{key}=true"
        | :? bool as b when not b -> Some $"{key}=false"
        | :? System.DateTime as d when d <> System.DateTime.MinValue -> 
            let str=d.ToString("yyyy-MM-dd")
            Some $"{key}={str}"
        | :? Option<_> as o ->
            match o with
            | Some v -> queryValue key v
            | None -> None
        | _ -> None

    let buildQuery (parameters: (string * obj) list) =
        parameters
        |> List.choose (fun (k, v) -> queryValue k v)
        |> String.concat "&"
        |> fun s -> if s = "" then "" else "?" + s


    [<Rpc>]
    let getUsers():Async<User list> =
        async {
            use client = new HttpClient()
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let response = client.GetStringAsync(apiUrl+"user") |> Async.AwaitTask |> Async.RunSynchronously
            return JsonConvert.DeserializeObject<User list>(response)
        }

    [<Rpc>]
    let getRegistries():Async<Registry list> =
        async {
            use client = new HttpClient()
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let response = client.GetStringAsync(apiUrl+"registries") |> Async.AwaitTask |> Async.RunSynchronously
            return JsonConvert.DeserializeObject<Registry list>(response)
        }

    [<Rpc>]
    let getUser(id:int):Async<User> =
        async {
            use client = new HttpClient()
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let response = client.GetStringAsync(apiUrl+"user/"+id.ToString()) |> Async.AwaitTask |> Async.RunSynchronously
            return JsonConvert.DeserializeObject<User>(response)
        }

    [<Rpc>]
    let getRegistry(id:int):Async<Registry> =
        async {
            use client = new HttpClient()
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let request = new HttpRequestMessage(HttpMethod.Get, apiUrl + "registries/" + id.ToString())
            let response = client.SendAsync(request) |> Async.AwaitTask |> Async.RunSynchronously
            if response.IsSuccessStatusCode then
                let content = response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
                return JsonConvert.DeserializeObject<Registry>(content)
            else
                return {id=0;date=System.DateTime.MinValue;ownerid=0;text=""}
        }

    [<Rpc>]
    let getUserParams(name:string,password:string):Async<User list> =
        let query =
            buildQuery [
                "name", box name
                "password", box password
            ]
        async {
            use client = new HttpClient()
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let request = new HttpRequestMessage(HttpMethod.Get, apiUrl + "user/" + query)
            let response = client.SendAsync(request) |> Async.AwaitTask |> Async.RunSynchronously
            if response.IsSuccessStatusCode then
                let content = response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
                return JsonConvert.DeserializeObject<User list>(content)
            else
                return []
        }

    [<Rpc>]
    let getRegistryParams(date:System.DateTime,ownerid:int,text:string):Async<Registry list> =
        let query =
            buildQuery [
                "date", box date
                "ownerid", box ownerid
                "text", box text
            ]
        async {
            use client = new HttpClient()
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let request = new HttpRequestMessage(HttpMethod.Get, apiUrl + "registries/" + query)
            let response = client.SendAsync(request) |> Async.AwaitTask |> Async.RunSynchronously
            if response.IsSuccessStatusCode then
                let content = response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
                return JsonConvert.DeserializeObject<Registry list>(content)
            else
                return []
        }

    [<Rpc>]
    let createUser(user:User):Async<Create> =
        async {
            use client = new HttpClient()
            let mess=new HttpRequestMessage(HttpMethod.Put,Uri(apiUrl+"user/"))
            let json=JsonConvert.SerializeObject(user)
            mess.Content <- new StringContent(json, Text.Encoding.UTF8, "application/json")
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let res = client.SendAsync(mess) |> Async.AwaitTask |> Async.RunSynchronously
            let content = res.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
            return JsonConvert.DeserializeObject<Create>(content)
        }

    [<Rpc>]
    let createRegistry(registry:Registry):Async<Create> =
        async {
            use client = new HttpClient()
            let mess=new HttpRequestMessage(HttpMethod.Put,Uri(apiUrl+"registries/"))
            let json=JsonConvert.SerializeObject(registry)
            mess.Content <- new StringContent(json, Text.Encoding.UTF8, "application/json")
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let res = client.SendAsync(mess) |> Async.AwaitTask |> Async.RunSynchronously
            let content = res.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
            return JsonConvert.DeserializeObject<Create>(content)
        }

    [<Rpc>]
    let updateUser(newUser:User,id:int):Async<Update> =
        async {
            use client = new HttpClient()
            let mess=new HttpRequestMessage(HttpMethod.Patch,Uri(apiUrl+"user/"+id.ToString()))
            let json=JsonConvert.SerializeObject(newUser)
            mess.Content <- new StringContent(json, Text.Encoding.UTF8, "application/json")
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let res = client.SendAsync(mess) |> Async.AwaitTask |> Async.RunSynchronously
            let content = res.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
            return JsonConvert.DeserializeObject<Update>(content)
        }

    [<Rpc>]
    let updateRegistry(newRegistry:Registry,id:int):Async<Update> =
        async {
            use client = new HttpClient()
            let mess=new HttpRequestMessage(HttpMethod.Patch,Uri(apiUrl+"registries/"+id.ToString()))
            let json=JsonConvert.SerializeObject(newRegistry)
            mess.Content <- new StringContent(json, Text.Encoding.UTF8, "application/json")
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let res = client.SendAsync(mess) |> Async.AwaitTask |> Async.RunSynchronously
            let content = res.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
            return JsonConvert.DeserializeObject<Update>(content)
        }

    [<Rpc>]
    let deleteUser(id:int):Async<Update> =
        async {
            use client = new HttpClient()
            let mess=new HttpRequestMessage(HttpMethod.Delete,Uri(apiUrl+"user/"+id.ToString()))
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let res = client.SendAsync(mess) |> Async.AwaitTask |> Async.RunSynchronously
            let content = res.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
            return JsonConvert.DeserializeObject<Update>(content)
        }

    [<Rpc>]
    let deleteRegistry(id:int):Async<Update> =
        async {
            use client = new HttpClient()
            let mess=new HttpRequestMessage(HttpMethod.Delete,Uri(apiUrl+"registries/"+id.ToString()))
            client.DefaultRequestHeaders.Add("akey", apiKey)
            let res = client.SendAsync(mess) |> Async.AwaitTask |> Async.RunSynchronously
            let content = res.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
            return JsonConvert.DeserializeObject<Update>(content)
        }