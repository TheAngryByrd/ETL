open FSharpx.Control
open System
open System.Reactive.Linq
open System.Reactive.Threading.Tasks
open System.Threading
open Loaders
open Chessie.ErrorHandling


module Observable =
    let ofAsync async =    
        let task = async |> Async.StartAsTask
        task.ToObservable()


let batcher = new BatchProcessingAgent<string>(10000, 1000)
let stream = Console.OpenStandardOutput()

batcher.BatchProduced
    .SelectMany(fun items -> sendManyViaStream stream items |> Observable.ofAsync)
    .SubscribeOn(System.Reactive.Concurrency.ThreadPoolScheduler.Instance)
    .ObserveOn(System.Reactive.Concurrency.ThreadPoolScheduler.Instance)
    .Add(fun result -> 
            match result with 
            | Pass result -> ()
            | Warn (result, messages) ->()
            | Fail messages -> ()
        )

[<EntryPoint>]
let main argv = 
    //printfn "%A" argv
    use cts = new CancellationTokenSource()
        
    for i in 1..100000 do
        i.ToString() + "\n" |> batcher.Enqueue
    Console.ReadLine() |> ignore
    0 // return an integer exit code
