namespace ETL.Tests

module Tests =

    open Loaders
    open System.IO
    open Xunit
    open FsCheck
    open FsCheck.Xunit
    open Swensen.Unquote
    open System.Text
    open FSharpx.Control
    open System.Threading
    open System
    open Chessie.ErrorHandling
    open System.Net.Sockets
    open System.Net
    open Domain

    [<Property>]
    let ``Send output via memorystream`` (input : string) = // Async.StartAsTask <| async {
        input <> null ==> lazy
            use memorystream = new MemoryStream()

            let rs = sendViaStream memorystream input |> Async.RunSynchronously
                           
            let dataInString = memorystream.ToArray() |> Encoding.UTF8.GetString
            input = dataInString && ok(input) = rs


    [<Property>]
    let ``Send output via tcpstream`` (input : string) =
        input <> null ==> lazy
            let localEndPoint = IPEndPoint(IPAddress.Loopback, 11000)

            use socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
            socket.Bind(localEndPoint)
            socket.Listen(1)
            
            use tcpClient = new TcpClient()
            tcpClient.Connect(localEndPoint)
            let stream = tcpClient.GetStream()

            let rs = sendViaStream stream input |> Async.RunSynchronously

            ok(input) = rs

    [<Property>]
    let ``Send output to disconnected tcpstream`` (input : string) = 
        input <> null ==> lazy            
            let localEndPoint = IPEndPoint(IPAddress.Loopback, 11000)

            use socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
            socket.Bind(localEndPoint)
            socket.Listen(1)
            
            use tcpClient = new TcpClient()
            tcpClient.Connect(localEndPoint)
            use stream = tcpClient.GetStream()
            socket.Dispose()  //Simulate network interrupt

            let rs = sendViaStream stream input |> Async.RunSynchronously
                           

            fail(CantWriteToStream) = rs

//    let split  (splitter : char)  (str : string)=
//        str.Split([|splitter|], StringSplitOptions.RemoveEmptyEntries)

//    [<Fact>]
//    let ``Send batches`` () =        
//        let memorystream = new MemoryStream()
//        let sendViaMemoryStream = sendViaStream memorystream
//        let batcher = new BatchProcessingAgent<string>(10, 1000)
//        
//        let length = 1000
//
//        batcher.BatchProduced.Add(fun items -> items |> Seq.map sendViaMemoryStream |> Async.Parallel |> Async.Ignore |> Async.RunSynchronously)
//        
//        async {
//            for _ in 1 .. length do 
//                batcher.Enqueue("Jimmy\n") } |> Async.RunSynchronously
//
//        Thread.Sleep(100)
//        let x = memorystream.ToArray() |> Encoding.UTF8.GetString |> split '\n' 
//        
//        test <@ x.Length = length @>
//        ()
        


