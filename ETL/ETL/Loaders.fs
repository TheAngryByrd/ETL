module Loaders 
open System.IO
open System.Text

let sendViaStream (stream : Stream) (output : string) = async {
    if output <> null then
        do! output |> Encoding.UTF8.GetBytes |> stream.AsyncWrite
}

