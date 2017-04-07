module EventStore

open System
open EventStore.ClientAPI

let eventData (event: ResolvedEvent) =
    event.Event.EventType, event.Event.Data

let toEventData (typeName, data) =
    EventData(Guid.NewGuid(), typeName, true, data, null)

let readEvents<'e> (store: IEventStoreConnection) streamId version =
    let rec loop version previousEvents =
        async {
            let! slice = store.ReadStreamEventsForwardAsync(streamId, version, 4000, true) |> Async.AwaitTask
            
            let events = 
                slice.Events
                |> Seq.collect (eventData >> Serialization.deserialize<'e>)
            
            if slice.IsEndOfStream then
                return slice.LastEventNumber, Seq.append events previousEvents   |> Seq.toList
            else
                return! loop slice.NextEventNumber (Seq.append events previousEvents)
        }
    loop version Seq.empty


let appendEvents<'e> (store: IEventStoreConnection) streamId expectedVersion (events: 'e list) =
    async {
        let eventData =
            events
            |> List.map (Serialization.serialize >> toEventData)
            |> List.toArray
            
        let! result = store.AppendToStreamAsync(streamId, expectedVersion, eventData) |> Async.AwaitTask
        return result.NextExpectedVersion
    }