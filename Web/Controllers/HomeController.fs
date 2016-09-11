namespace Web.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax
open System.Net.Mime
open System.Web.Http

type HomeController() =
    inherit Controller()
    member this.Index () = this.View()

    [<HttpPost>]
    member this.Generate (score:string) = 
        match Assembler.assembleToPackedStream score with
            | Choice1Of2 res -> 
                this.Response.AppendHeader("Content-Disposition", ContentDisposition(FileName="ring.wav", Inline=false).ToString())
                res.Position <- 0L
                this.File(res, "audio/x-wav")
            | Choice2Of2 err -> failwith err

