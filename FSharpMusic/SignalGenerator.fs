module SignalGenerator

let generateSamples miliseconds frequency = 
    let sampleRate = 44100.0
    let sizteenBitSampleLimit = 32767.0
    let volume = 0.8
    let toAmplitude x = 
        x
        |> (*) (2.0 * System.Math.PI * frequency / sampleRate)
        |> sin
        |> (*) sizteenBitSampleLimit
        |> (*) volume
        |> int16
    
    let numOfSamples = miliseconds / 1000.0 * sampleRate
    let requiredSamples = seq { 1.0..numOfSamples }

    Seq.map toAmplitude requiredSamples