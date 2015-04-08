open FSharpx.Control
open System
open System.Reactive.Linq
open System.Reactive.Threading.Tasks
open System.Threading
open Loaders


module Observable =


let batcher = new BatchProcessingAgent<string>(10000, 1000)
let stream = Console.OpenStandardOutput()
let batchProducer =batcher.BatchProduced :> IObservable<_> 
batchProducer
    .SelectMany(fun items -> sendManyViaStream stream items |> Observable.ofAsync)
    .SubscribeOn(System.Reactive.Concurrency.ThreadPoolScheduler.Instance)
    .ObserveOn(System.Reactive.Concurrency.ThreadPoolScheduler.Instance)
    .Add(fun result -> ())

[<EntryPoint>]
let main argv = 
    //printfn "%A" argv
    use cts = new CancellationTokenSource()
    for i in 1..1000000 do
        i.ToString() + "\n" |> batcher.Enqueue
    Console.ReadLine() |> ignore
    0 // return an integer exit code
