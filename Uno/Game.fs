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
    | WrongCardPlayed of CardPlayed
    | PlayerPlayedAtWrongTurn of CardPlayed
    | InterruptMissed of CardPlayed
    | TurnStarted of TurnStarted

and GameStarted = {
    PlayerCount: int
    FirstCard: Card
}
and CardPlayed = {
    Player: int
    Card: Card
}
and TurnStarted = {
    Player: int
}

type GameError = 
    | TooFewPlayers
    | GameAlreadyStarted
    | GameNotStarted


// This is the game state
type State = 
    | InitialState
    | Started of Started
and Started = {
    Players: int
    CurrentPlayer: int
    TopCard: Card
    CardUnder: Card option
}

// Step 1:
// Make the simplest implementation for the following signature
// Command -> State -> Event list Result
let decide command state =
    match command, state with
    | StartGame cmd, _ when cmd.PlayerCount < 2 ->
        Error TooFewPlayers
    | StartGame cmd, InitialState -> 
        Ok [ GameStarted { 
               PlayerCount = cmd.PlayerCount
               FirstCard = cmd.FirstCard }
             TurnStarted { Player = 1 }
               ]
    | StartGame _, Started _ ->
        Error GameAlreadyStarted
    | PlayCard _, InitialState ->
        Error GameNotStarted
    | PlayCard cmd, Started s when 
            cmd.Player <> s.CurrentPlayer 
            && cmd.Card <> s.TopCard ->
        if Some cmd.Card = s.CardUnder then
            Ok [ InterruptMissed {
                Player = cmd.Player
                Card = cmd.Card }]
        else
            Ok [ PlayerPlayedAtWrongTurn {
                Player = cmd.Player
                Card = cmd.Card }]
    | PlayCard cmd, Started s when 
            cmd.Player <> s.CurrentPlayer 
            && cmd.Card = s.TopCard ->
        Ok [ CardPlayed {
                Player = cmd.Player
                Card = cmd.Card
            }]
    | PlayCard cmd, Started s->
        match cmd.Card, s.TopCard with
        | Digit(c1,d1), Digit(c2,d2) when c1 = c2 || d1 = d2 ->
            Ok [ CardPlayed {
                    Player = cmd.Player
                    Card = cmd.Card }
                 TurnStarted {
                     Player = (s.CurrentPlayer + 1) % s.Players
                 }]
        | _ -> Ok [ WrongCardPlayed {
                    Player = cmd.Player
                    Card = cmd.Card }]
    | _ -> Ok []

// Step 2:
// Make the simplest implementation for the following signature
// State -> Event -> State
let evolve state event =
    match state, event with
    | _, GameStarted s -> 
        Started { 
            Players = s.PlayerCount
            CurrentPlayer = 1
            TopCard = s.FirstCard
            CardUnder = None }
    | Started s, TurnStarted e -> 
        Started { 
            s with
                CurrentPlayer = e.Player }
    | Started s, CardPlayed e ->
        Started {
            s with
                TopCard = e.Card
                CardUnder = Some s.TopCard }
    | _ -> state

