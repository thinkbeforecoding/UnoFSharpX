[<AutoOpen>]
module Model

// type Result<'a,'b> =
//     | Ok of 'a
//     | Error of 'b



type Color = Red | Green | Blue | Yellow

type Digit = Zero | One | Two | Three | Four | Five | Six | Seven | Height | Nine

type Card =
    | Digit of Color * Digit
    // | KickBack of Color
    // | Jump of Color   

