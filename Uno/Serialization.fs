module Serialization

open Game
open Newtonsoft.Json // nuget : Newtonsoft.Json
open FSharp.Reflection
open System

type SingleCaseUnionConverter() =
    inherit JsonConverter()

    override __.CanConvert t =
        if FSharpType.IsUnion t then
            match FSharpType.GetUnionCases(t) with
            | [| case |] when case.GetFields().Length =1  -> 
                true 
            | _ -> false
        else
            false  
    override __.CanRead = true
    override __.CanWrite = true

    override __.WriteJson(w,o,s) = 
        let case, fields = FSharpValue.GetUnionFields(o, o.GetType())
        let t = case.GetFields().[0].PropertyType
        s.Serialize(w, fields.[0], t)

    override __.ReadJson(r,t,_,s) =
        let cases = FSharpType.GetUnionCases t 
        let case = cases.[0]
        let field = case.GetFields().[0]
        let v = s.Deserialize(r, field.PropertyType)
        FSharpValue.MakeUnion(case, [|v|])


type UnionConverter() =
    inherit JsonConverter()

    let isList (t: Type) =
        t.IsGenericType
        && t.GetGenericTypeDefinition() = typedefof<_ list> 

    override __.CanConvert t =
        FSharpType.IsUnion t && not (isList t)
    override __.CanRead = true
    override __.CanWrite = true

    override __.WriteJson(w,o,s) = 
        let case, fields = FSharpValue.GetUnionFields(o, o.GetType())
        if fields.Length = 0 then
            w.WriteValue(case.Name)
        else
            w.WriteStartObject()
            w.WritePropertyName(case.Name)
            if fields.Length = 1 then
                let t = case.GetFields().[0].PropertyType
                s.Serialize(w, fields.[0], t)

            else
                w.WriteStartArray()
    
                for field, t in Array.zip fields (case.GetFields()) do
                    s.Serialize(w, field, t.PropertyType)
                w.WriteEndArray()
            w.WriteEndObject()
        
    override __.ReadJson(r,t,_,s) =
        let cases = FSharpType.GetUnionCases t 
        if r.TokenType = JsonToken.String then
            let name = string r.Value
            r.Read() |> ignore

            let case = cases |> Array.find (fun c -> c.Name = name) 
            FSharpValue.MakeUnion(case, null)
        else 
            r.Read() |> ignore
            let name = string r.Value
            let case = cases |> Array.find (fun c -> c.Name = name) 
            let fields = case.GetFields()
            r.Read() |> ignore
            let value = 
                if fields.Length = 1 then
                    let v = s.Deserialize(r, fields.[0].PropertyType)
                    FSharpValue.MakeUnion(case, [|v|])
                else 
                    r.Read() |> ignore
                    let values =
                        fields
                        |> Array.map (fun f -> 
                            s.Deserialize(r, f.PropertyType))
                    r.Read() |> ignore
                    FSharpValue.MakeUnion(case, values)
            value 

let serializer =
    JsonSerializer.Create(
        JsonSerializerSettings(
            Converters = [|
                SingleCaseUnionConverter()
                UnionConverter()
            |]
        ))

let serializeObj o =
    use stream = new IO.MemoryStream()
    use streamWriter = new IO.StreamWriter(stream)
    use writer = new JsonTextWriter(streamWriter)
    // use writer = new IO.StringWriter()
    serializer.Serialize(writer, o)
    writer.Flush()
    stream.ToArray()

let deserializeObj objType data  =
    use stream = new IO.MemoryStream(data: byte[])
    use streamReader = new IO.StreamReader(stream, true)
    use reader = new JsonTextReader(streamReader)
    serializer.Deserialize(reader, objType)


let serialize (event:'e) = 
    let case, values = FSharpValue.GetUnionFields(event, typeof<'e>)
    let data = serializeObj values.[0]
    case.Name, data
        

let deserialize<'e> (eventType, data): 'e list =
    FSharpType.GetUnionCases(typeof<'e>)
    |> Array.tryFind (fun c -> c.Name = eventType)
    |> function
        | Some case ->
                let fieldType = case.GetFields().[0].PropertyType
                [ 
                    FSharpValue.MakeUnion(
                        case, 
                        [| deserializeObj fieldType data |])
                    |> unbox 
                    ]
        | None -> []
