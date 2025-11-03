#r "nuget: FSharp.Collections.ParallelSeq"

open FSharp.Collections.ParallelSeq

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