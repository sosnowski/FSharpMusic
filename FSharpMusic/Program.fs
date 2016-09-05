
// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System.IO

[<EntryPoint>]
let main argv = 
    printfn "%A" argv

    let write fileName (ms: MemoryStream) =
        use fs = new FileStream(Path.Combine(".", fileName), FileMode.Create)
        ms.WriteTo(fs)

    Array.ofSeq (SignalGenerator.generateSamples 5000.0 440.0)
    |> WavePacker.pack
    |> write "test.wav"

    0 // return an integer exit code
