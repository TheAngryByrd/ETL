module Loaders 
open System.IO
open System.Text
open Chessie.ErrorHandling
open Domain

let sendViaStream (stream : Stream) (output : string) = async {
    try    
        do! output |> Encoding.UTF8.GetBytes |> stream.AsyncWrite 
        return ok(output)
    with | ex -> return fail(CantWriteToStream)
}

