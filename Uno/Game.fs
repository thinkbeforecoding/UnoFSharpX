module Game

// This is a Command
// It is represented by verbs at the imperative tense
type Command =
    | StartGame of StartGame
    | PlayCard of PlayCard

and StartGame = {
    PlayerCount: int
    FirstCard: Card
}
and PlayCard = {
    Player: int
    Card: Card
}

// This is an Event
// It is represented by verbs at the past tense
type Event =
    | GameStarted of GameStarted
    | CardPlayed of CardPlayed

and GameStarted = {
    PlayerCount: int
    FirstCard: Card
}
and CardPlayed = {
    Player: int
    Card: Card
}

type GameError = 
    | TooFewPlayers
    | GameAlreadyStarted
    | GameNotStarted


// This is the game state
type State = 
    | InitialState

// Step 1:
// Make the simplest implementation for the following signature
// Command -> State -> Event list Result
let decide command state = failwith "not implemented"

// Step 2:
// Make the simplest implementation for the following signature
// State -> Event -> State
let evolve state event = failwith "not implemented"

