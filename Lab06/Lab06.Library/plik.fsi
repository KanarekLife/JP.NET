namespace FSharp




module Project.fsproj


module Script

val y: int

val data: float list

val inline sqr: x: ^a -> 'b when ^a: (static member ( * ) : ^a * ^a -> 'b)

val sumOfSquaresI: nums: int seq -> int

val sumOfSquaresF: nums: float list -> float

val sumOfSquares: nums: int seq -> int

val sumOfSquaresP: nums: float seq -> float

val loadPrices: ticker: string -> (string * string) array

val loadPricesAsync: ticker: string -> Async<(string * string) array>

val requests: Async<(string * string) array> list

val parallelRequests: Async<(string * string) array array>

val results: (string * string) array array

type Adder = int -> int

type AdderGenerator = int -> int -> int

val a: x: int -> y: int -> int

val b: x: int -> y: int -> int

val c: x: float -> y: float -> float

namespace Lab06.Library
    
    type StockAnalyzer =
        
        new: lprices: (string * string) array * days: int -> StockAnalyzer
        
        static member GetAnalyzer: ticker: string * days: int -> StockAnalyzer
        
        static member
          GetAnalyzers: tickers: string seq * days: int -> StockAnalyzer seq
        
        static member
          GetAnalyzersParal: tickers: string seq * days: int ->
                               StockAnalyzer seq
        
        static member
          GetAnalyzersParalAsync: tickers: string seq * days: int ->
                                    Async<StockAnalyzer array>
        
        static member
          GetAnalyzersParalTask: tickers: string seq * days: int ->
                                   System.Threading.Tasks.Task<StockAnalyzer array>
        
        member Return: float
        
        member StdDev: float

