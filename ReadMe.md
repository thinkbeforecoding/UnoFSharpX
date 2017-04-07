# Uno Workshop for FSharpX

This is the code we will start with for the FSharpX Functional Event Sourcing workshop.

Just follow the steps in order !

Have <| fun _ -> ()

## Installing the EventStore

Go to the [download page](https://geteventstore.com/downloads/) and download the appropriate version.

Unzip it.

Run it in memory with running projection:

    ./EventStore.ClusterNode.exe --mem-db --run-projections System --start-standard-projections