
// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System.IO

[<EntryPoint>]
let main argv = 
    printfn "%A" argv

    let write fileName (ms: MemoryStream) =
        use fs = new FileStream(Path.Combine(".", fileName), FileMode.Create)
        ms.WriteTo(fs)

//    Array.ofSeq (SignalGenerator.generateSamples 5000.0 440.0)
//    |> WavePacker.pack
//    |> write "test.wav"


    let test = "8#g2 8e2 8#g2 8#c3 4a2 4- 8#f2 8#d2 8#f2 8b2 4#g2 8#f2 8e2 4- 8e2 8#c2 4#f2 4#c2 4- 8#f2 8e2 4#g2 4#f2"

    match Assembler.assembleToPackedStream test with
        | Choice1Of2 res -> res |> write "testStream.wav"
        | Choice2Of2 err -> failwith err

    0 // return an integer exit code
