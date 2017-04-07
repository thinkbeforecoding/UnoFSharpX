module Uno

open System
open EventStore.ClientAPI
open CommandHandler
open Game

[<EntryPoint>]
let main argv =

    async {
        use store = EventStoreConnection.Create(Uri "tcp://127.0.0.1:1113")
        do! store.ConnectAsync() |> Async.AwaitTask

        let gameStream (GameId id) = sprintf "Game-%d" id

        let readEvents id = EventStore.readEvents<Event> store (gameStream id)
        let appendEvents id = EventStore.appendEvents<Event> store (gameStream id)
        let handle' = handle readEvents appendEvents 
        let id = GameId 2

        do! handle' id (StartGame { PlayerCount = 4; FirstCard = Digit(Red, Three) })
        do! handle' id (PlayCard { Player = 1; Card = Digit(Red, Seven) })


    } |> Async.RunSynchronously

    0 // return an integer exit code
