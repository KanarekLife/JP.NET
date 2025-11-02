Imports System
Imports System.Threading.Tasks
Imports Lab06.Library

Module Program
    Sub Main(args As String())
        MainAsync(args).GetAwaiter().GetResult()
    End Sub

    Private Async Function MainAsync(args As String()) As Task
        Dim tickers = {"MSFT", "ORCL", "EBAY"}
        Dim days = 30

        Console.WriteLine("Pobieranie danych i analiza asynchroniczna...")
        Dim analyzers = Await StockAnalyzer.GetAnalyzersParalTask(tickers, days)

        For i = 0 To tickers.Length - 1
            Dim a = analyzers(i)
            Dim ret = a.Return
            Dim sd = a.StdDev
            Console.WriteLine($"{tickers(i)} -> Stopa zwrotu: {ret:P2}, Odchylenie standardowe: {sd:P4}")
        Next
    End Function
End Module
