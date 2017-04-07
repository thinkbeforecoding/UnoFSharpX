module CommandHandler

open Game

// [<Struct>]
type GameId = GameId of int

// Step 16:
// Implement the command handler
let handle readEvents appendEvents id command =
    async {

        let! version, events = readEvents id 0L
        let newEvents =
            events
            |> List.fold evolve InitialState
            |> decide command

        match newEvents with
        | Ok newEvents ->
            do! appendEvents id version newEvents |> Async.Ignore

        | Error err ->
            failwithf "%A" err
    }

// Step 17:
// Add keep state in memory
// Step 18:
// Implement Snapshoting

// type Agent<'t> = MailboxProcessor<'t>


// let startGame readEvents appendEvents readSnapshot saveSnapshot  id =
//     Agent.Start <| fun mailbox ->
//         let rec loop (version,state) snapVersion =
//             async {
//                 let! command = mailbox.Receive()
            
//                 let newEvents = decide command state
//                 match newEvents with
//                 | Ok newEvents ->
//                     let! nextVersion =
//                         appendEvents id version newEvents

//                     let newState = List.fold evolve state newEvents
//                     if nextVersion - snapVersion > 100 then
//                         do! saveSnapshot id nextVersion newState 
//                         return! loop (nextVersion, newState) nextVersion
//                     else
//                         return! loop (nextVersion, newState) snapVersion
//                 | Error err ->
//                     return failwithf "%A" err 
//         }
//         async {
//             let! snapVersion, state = readSnapshot id

//             let! version, events = readEvents id snapVersion
//             let state = List.fold evolve state events
//             return! loop (version, state) version

//         }

// let handle readEvents appendEvents readSnapshot saveSnapshot id command =
//     async {
//         let! snapVersion, state = readSnapshot id

//         let! version, events = readEvents id snapVersion
//         let newEvents =
//             events
//             |> List.fold evolve state
//             |> decide command

//         match newEvents with
//         | Ok newEvents ->
//             do! appendEvents id version newEvents |> Async.Ignore

//             if version - snapVersion > 100L then
//                 do! saveSnapshot id version state

//         | Error err ->
//             failwithf "%A" err
//     }


