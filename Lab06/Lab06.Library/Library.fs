namespace Lab06.Library

open Script
open System
open System.Globalization
open System.Threading.Tasks

type public StockAnalyzer (lprices, days) =
    let prices =
        lprices
        |> Seq.map snd
        |> Seq.take days
    
    static member public GetAnalyzer(ticker, days) =
        new StockAnalyzer(loadPrices ticker, days)
    
    static member public GetAnalyzers(tickers, days) =
        tickers
        |> Seq.map loadPrices
        |> Seq.map (fun prices -> new StockAnalyzer(prices, days))
    
    member s.Return =
        let lastPriceString = prices |> Seq.item 0
        let startPriceString = prices |> Seq.item (days - 1)
        let lastPrice = Double.Parse(lastPriceString, CultureInfo.InvariantCulture)
        let startPrice = Double.Parse(startPriceString, CultureInfo.InvariantCulture)
        lastPrice / startPrice - 1.0
    
    member s.StdDev =
        let logRets =
            prices
            |> Seq.pairwise
            |> Seq.map (fun (yStr, xStr) ->
                let x = Double.Parse(xStr, CultureInfo.InvariantCulture)
                let y = Double.Parse(yStr, CultureInfo.InvariantCulture)
                log (y / x))
        let mean = logRets |> Seq.average
        let sqr x = x * x
        let var = logRets |> Seq.averageBy (fun r -> sqr (r - mean))
        sqrt var
    
    static member public GetAnalyzersParal (tickers, days) =
        tickers
        |> Seq.map loadPricesAsync
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Seq.map (fun prices -> new StockAnalyzer(prices, days))
        
    static member public GetAnalyzersParalAsync (tickers, days) = async {
        let operations =
            tickers |> Seq.map loadPricesAsync
        let parallelOperations = Async.Parallel operations
        let! allPrices = parallelOperations
        let analyzers =
            allPrices
            |> Array.map (fun prices -> new StockAnalyzer(prices, days))
        return analyzers
    }

    static member public GetAnalyzersParalTask (tickers, days) : Task<StockAnalyzer[]> =
        StockAnalyzer.GetAnalyzersParalAsync(tickers, days)
        |> Async.StartAsTask
        