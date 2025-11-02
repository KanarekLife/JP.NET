module Script

#r "nuget: FSharp.Collections.ParallelSeq"
#r "nuget: XPlot.Plotly"

open System
open FSharp.Collections.ParallelSeq
open XPlot.Plotly
open System.Net.Http

let y = 0
let data = [1.;2.;3.;4.]
let inline sqr x = x * x

let sumOfSquaresI nums =
    let mutable acc = 0
    for x in nums do
        acc <- acc + sqr x
    acc

let rec sumOfSquaresF nums =
    match nums with
    | [] -> 0.
    | h::t -> sqr h + sumOfSquaresF t
//
// let sumOfSquares nums =
//     Seq.sum(Seq.map(fun x -> x * x) nums)
    
let sumOfSquares nums =
    nums
    |> Seq.map(fun x -> x * x)
    |> Seq.sum

let sumOfSquaresP (nums: seq<float>) =
    nums
    |> PSeq.map(fun x -> x * x)
    |> PSeq.sum

let loadPrices (ticker: string) =
    let ticker = ticker.ToLower() + ".us"
    let dataStart = DateTime(2000, 1, 1).ToString("yyyyMMdd")
    let dataEnd = DateTime(2023, 2, 10).ToString("yyyyMMdd")
    let url = sprintf "https://stooq.com/q/d/l/?s=%s&d1=%s&d2=%s&i=d" ticker dataStart dataEnd
    let client = new HttpClient()
    let getDataAsync = async {
        let! csvResponse = Async.AwaitTask (client.GetStringAsync(url))
        return csvResponse
    }
    let csv = Async.RunSynchronously getDataAsync
    let prices =
        csv.Split([|'\r'; '\n'|], StringSplitOptions.RemoveEmptyEntries)
        |> Seq.skip 1
        |> Seq.map (fun line -> line.Split(','))
        |> Seq.filter (fun values -> values.Length = 6)
        |> Seq.map (fun values -> (values.[0], values.[4]))
        |> Seq.toArray
    prices

["MSFT"; "ORCL"; "EBAY"] |> Seq.iter (fun ticker ->
    let data = loadPrices ticker
    Chart.Line(data).Show()
)


let loadPricesAsync (ticker: string) = async {
    let ticker = ticker.ToLower() + ".us"                                                                      
    let dataStart = DateTime(2000, 1, 1).ToString("yyyyMMdd")                                                  
    let dataEnd = DateTime(2023, 2, 10).ToString("yyyyMMdd")                                                   
    let url = sprintf "https://stooq.com/q/d/l/?s=%s&d1=%s&d2=%s&i=d" ticker dataStart dataEnd                 
    let client = new HttpClient()                                                                                                                                                                                                          
    let! csv = Async.AwaitTask (client.GetStringAsync(url))                                                              
    let prices =                                                                                               
        csv.Split([|'\r'; '\n'|], StringSplitOptions.RemoveEmptyEntries)
        |> PSeq.ofArray
        |> PSeq.skip 1                                                                                           
        |> PSeq.map (fun line -> line.Split(','))                                                                
        |> PSeq.filter (fun values -> values.Length = 6)                                                         
        |> PSeq.map (fun values -> (values.[0], values.[4]))                                                     
        |> PSeq.toArray                                                                                          
    return prices                                                                                                     
}
let requests = [
    loadPricesAsync "MSFT"
    loadPricesAsync "ORCL"
]
let parallelRequests = Async.Parallel requests
let results = Async.RunSynchronously parallelRequests
results |> Array.iter (fun data ->
    Chart.Line(data).Show()    
)

type Adder = int -> int
type AdderGenerator = int -> Adder
let a: AdderGenerator = fun x -> (fun y -> x + y)
let b:AdderGenerator = fun (x:int) -> (fun y -> x + y)
let c = fun (x:float) -> (fun y -> x + y)


let notSoLongString = String50.create "notSoLongString"
let toolongString = String50.create
"tooLongLongLongLongLongLongLongLongLongLongLongLongLongString"