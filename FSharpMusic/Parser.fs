module Parser

open FParsec

type MeasureFraction = Half | Quater | Eighth | Sixteenth | Thirtyseconth
type Length = { fraction: MeasureFraction; extended: bool }

type Note = A | ASharp | B | C | CSharp | D | DSharp | E | F | FSharp | G | GSharp
type Octave = One | Two | Three
type Sound = Rest | Tone of note: Note * octave: Octave

type Token = { length: Length; sound: Sound }

let testValue = "32.#d3"

let pMeasureFraction = 
    (stringReturn "2" Half)
    <|> (stringReturn "4" Quater)
    <|> (stringReturn "8" Eighth) 
    <|> (stringReturn "16" Sixteenth) 
    <|> (stringReturn "32" Thirtyseconth)

let pExtended = (stringReturn "." true) <|> (stringReturn "" false)

let pLength = pipe2 pMeasureFraction pExtended (fun res1 res2 -> { fraction = res1; extended = res2 })

let pNotSharpableNote = anyOf "be" |>> (fun res ->
    match res with
    | 'b' -> B
    | 'e' -> E
    | _ -> "Uknown note" |> failwith
)

let pSharp = (stringReturn "#" true) <|> (stringReturn "" false)

let pSharpNote = pipe2 pSharp (anyOf "acdfg") (fun isSharp note ->
    match (isSharp, note) with
    | (false, 'a') -> A
    | (true, 'a') -> ASharp
    | (false, 'c') -> C
    | (true, 'c') -> CSharp
    | (false, 'd') -> D
    | (true, 'd') -> DSharp
    | (false, 'f') -> F
    | (true, 'f') -> FSharp
    | (false, 'g') -> G
    | (true, 'g') -> GSharp
    | _ -> "Uknown note" |> failwith
)

let pNote = pNotSharpableNote <|> pSharpNote

let pOctave = anyOf "123" |>> (fun res ->
    match res with
    | '1' -> One
    | '2' -> Two
    | '3' -> Three
    | _ -> "Uknown octave" |> failwith
)

let pTone = pipe2 pNote pOctave ( fun note octave -> Tone(note = note, octave = octave))
let pRest = stringReturn "-" Rest

let pToken = pipe2 pLength (pRest <|> pTone) (fun length tone -> { length = length; sound = tone })

let pScore = sepBy pToken (pstring " ")


let parse score = 
    match run pScore score with
    | Success(result, _, _) -> Choice1Of2 result
    | Failure (errorMsg, _, _) -> Choice2Of2 errorMsg

let durationFromToken (token:Token) =
    let bitsPerMinute = 120.
    let secondsPerBit = 60.0/bitsPerMinute
    (match token.length.fraction with
        | Half -> 2.0 * 1000.0 * secondsPerBit
        | Quater -> 1. * 1000.0 * secondsPerBit
        | Eighth-> 0.5 * 1000.0 * secondsPerBit
        | Sixteenth -> 0.25 * 1000.0 * secondsPerBit
        | Thirtyseconth -> 1.0/8.0 * 1000.0 * secondsPerBit
    ) * (if token.length.extended then 1.5 else 1.0)

let octaveNumeric (octave) = 
    match octave with
        | One -> 1
        | Two -> 2
        | Three -> 3

let semitonesBetween lower upper =
    let noteSeq = [A;ASharp;B;C;CSharp;D;DSharp;E;F;FSharp;G;GSharp]
    let overAllIndex (note, octave) =
        let noteIndex = List.findIndex(fun n -> n = note) noteSeq
        noteIndex + ((octaveNumeric octave - 1) * 12)
    (overAllIndex upper) - (overAllIndex lower)


let frequency token =
    match token.sound with
        | Rest -> 0.0
        | Tone (note, octave) ->
            let gap = semitonesBetween (A, One) (note, octave)
            220.0 * ((2.0 * (1.0/12.0)) ** (float gap))