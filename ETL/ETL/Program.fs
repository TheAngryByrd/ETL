open FSharpx.Control
open System
open System.Reactive
open System.Reactive.Linq
open System.Reactive.Threading.Tasks
open System.Threading
open Loaders
open FSharp.Control.Reactive
open System.Threading.Tasks

// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.


module Observable =
    let ofAsync f =    
        let x = f |> Async.StartAsTask
        x.ToObservable()


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
    
    //Async.Start(processStreamQueue(stream), cts.Token)

    
    for i in 1..1000000 do
        //streamQueue.Add("Jimmy\n")
        i.ToString() + "\n" |> batcher.Enqueue
    Console.ReadLine() |> ignore
    0 // return an integer exit code
