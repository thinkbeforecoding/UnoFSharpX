module Tests

open Expecto
open Game

// Step 3:
// Implement => to make the test run
let (=>) events command = failwith "not implemented"
let (==) actual expected = Expect.equal actual (Ok expected) "Events should match" 

let (=!) actual error = Expect.equal actual (Error error) "Errors should match"

let digit c d = Digit(c,d) 

[<Tests>]
let tests =
  testList "samples" [

    // Step 4:
    // Change the decide function to make this test pass
    testCase "The game can be started in initial state" <| fun _ ->
      []
      => StartGame { PlayerCount = 2; FirstCard = digit Red Three }
      == GameStarted { PlayerCount = 2; FirstCard = digit Red Three}

    // Step 5:
    // Change the decide function to make this test pass
    testCase "Playing alone is not fun" <| fun _ ->
      []
      => StartGame { PlayerCount = 1; FirstCard = digit Yellow Seven}
      =! TooFewPlayers

    // Step 6:
    // What should you change to make this test pass ?
    testCase "Game should not be started twice" <| fun _ ->
      [GameStarted { PlayerCount = 3; FirstCard = digit Blue One}]
      => StartGame { PlayerCount = 4; FirstCard = digit Green Height}
      =! GameAlreadyStarted


    // Step 7:
    // Make this two tests pass... doing the simplest thing that work
    testCase "Card with same value can be played" <| fun _ ->
      [GameStarted { PlayerCount = 3; FirstCard = digit Red Five}]
      => PlayCard { Player = 1; Card = digit Green Five }
      == [ CardPlayed { Player = 1; Card = digit Green Five}]

    testCase "Card with same color can be played" <| fun _ ->
      [GameStarted { PlayerCount = 3; FirstCard = digit Red Five}]
      => PlayCard { Player = 1; Card = digit Red Nine }
      == [ CardPlayed { Player = 1; Card = digit Red Nine }]

    // Step 8:
    // Make this test pass
    testCase "Card can be played only once game is started" <| fun _ ->
      []
      => PlayCard {Player = 1; Card = digit Blue Six }
      =! GameNotStarted

    // Step 9:
    // What happens here ?!
    testCase "Card should be same color or same value" <| fun _ ->
      [GameStarted { PlayerCount = 3; FirstCard = digit Red Five }]
      => PlayCard {Player = 1; Card = digit Green Seven }
      // ...

    // Step 10:
    // What happens here ?!
    testCase "Player should play during his turn" <| fun _ ->
      [GameStarted { PlayerCount = 3; FirstCard = digit Red Five }]
      => PlayCard { Player = 2; Card = digit Red Height }
      // ..

    // Step 11:
    // Testing a full round
    testCase "The after a table round, the dealer plays" <| fun _ ->
      [ GameStarted { PlayerCount = 3; FirstCard = digit Red Five }
        CardPlayed { Player = 1; Card = digit Red Three }
        CardPlayed { Player = 2; Card = digit Blue Three}]
      => PlayCard { Player = 0; Card = digit Blue Six }
      == [ CardPlayed { Player = 0; Card = digit Blue Six }]
  
    // Step 12:
    // Look at the evolve function...
    // It starts to contains logic.
    // Try to remove the logic from the evolve function 
    // to put it back in the decide function 

    // Step 13:
    // Make this test pass
    testCase "Player can interrupt" <| fun _ ->
      [ GameStarted { PlayerCount = 3; FirstCard = digit Red Five }
        CardPlayed { Player = 1; Card = digit Red Three } ] 
      => PlayCard { Player = 0; Card = digit Red Three } // This is not Player 0's turn, but its same value *and* same color
      == [ CardPlayed { Player = 0; Card = digit Red Three }]

    // Step 14:
    // Missing an interrupt is not concidered as playing at the wrong turn.
    // So what happens here ?
    testCase "Player get no penalty when missing an interrupt" <| fun _ ->
      [ GameStarted { PlayerCount = 4; FirstCard = digit Red Five } // <- there are 4 players
        CardPlayed { Player = 1; Card = digit Red Three } // <- this is the card player 0 tries to interrupt
        CardPlayed { Player = 2; Card = digit Blue Three }] // <- but player 2 plays too fast
      => PlayCard { Player = 0; Card = digit Red Three } // <- it is not player's 0 turn, but we can see he tried to interrupt on previous card
      // ..


    // Step 15:
    // Uncomment the Kickback card and implement it.
    // The kickback changes the direction of the game.
    
  ]