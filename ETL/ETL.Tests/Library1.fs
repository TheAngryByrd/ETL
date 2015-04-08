namespace ETL.Tests

module Tests =

    open Loaders
    open System.IO
    open Xunit
    open FsCheck
    open FsCheck.Xunit
    open Swensen.Unquote
    open System.Text

    [<Property>]
    let ``Send output via stream`` (input : string) = // Async.StartAsTask <| async {
        input <> null ==> lazy
            let memorystream = new MemoryStream()
            sendViaStream memorystream input |> Async.RunSynchronously
               
            let dataInString = memorystream.ToArray() |> Encoding.UTF8.GetString
            input = dataInString
