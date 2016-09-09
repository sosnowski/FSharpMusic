module Assembler

open SignalGenerator
open Parser
open WavePacker


let tokenToSound token = 
    generateSamples(durationFromToken token) (frequency token)

let assemble tokens = 
    List.map tokenToSound tokens |> Seq.concat

let assembleToPackedStream (score:string) =
    match parse score with
        | Choice2Of2 errorMsg -> Choice2Of2 errorMsg
        | Choice1Of2 tokens -> assemble tokens |> Array.ofSeq |> pack |> Choice1Of2