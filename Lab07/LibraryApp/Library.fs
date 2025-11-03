namespace LibraryApp

open System
open System.IO
open System.Net.Http
open System.Text.Json
open System.Text.Json.Serialization
open FSharp.SystemTextJson

[<Measure>] type PLN
[<Measure>] type EUR
[<Measure>] type USD

// Discriminated union ensuring at least phone or address exists
type ContactInfo =
    | PhoneOnly of phone:string
    | AddressOnly of address:string
    | PhoneAndAddress of phone:string * address:string

[<CLIMutable>]
type DaneKontaktowe = {
    Imie: string
    Nazwisko: string
    DataUrodzenia: DateTime
    NrKartyBibliotecznej: string
    Email: string option
    Kontakt: ContactInfo
}

[<RequireQualifiedAccess>]
type StatusKonta =
    | Standard
    | Premium

[<CLIMutable>]
type Czytelnik = {
    Id: string
    Opis: DaneKontaktowe option
    KaucjaUSD: decimal<USD>
    DataDolaczenia: DateTime
    Status: StatusKonta
    KaryPLN: decimal<PLN>
}

// Loan history types
[<CLIMutable>]
type Loan = { ISBN: string; Returned: bool }

[<CLIMutable>]
type LoanHistory = { PatronId: string; Loans: Loan list }

module Serialization =
    let private jsonOptions =
        let o = JsonSerializerOptions(WriteIndented = true)
        o.Converters.Add(JsonStringEnumConverter())
        // F# options converter
        o.Converters.Add(JsonFSharpConverter())
        o

    let serialize<'a> (value: 'a) =
        JsonSerializer.Serialize(value, jsonOptions)

    let deserialize<'a> (json: string) =
        JsonSerializer.Deserialize<'a>(json, jsonOptions)

module Files =
    let patronsFile = Path.Combine("data", "patrons.json")
    let loansDir = Path.Combine("data", "loans")
    let loanFile patronId = Path.Combine(loansDir, sprintf "loans_%s.json" patronId)

    let ensureDataDirs () =
        Directory.CreateDirectory("data") |> ignore
        Directory.CreateDirectory(loansDir) |> ignore

module SampleData =
    let private samplePatrons : Czytelnik list =
        let kontakt1 = { Imie = "Anna"; Nazwisko = "Kowalska"; DataUrodzenia = DateTime(1990,1,1); NrKartyBibliotecznej = "A123"; Email = Some "anna@example.com"; Kontakt = PhoneOnly "600111222" }
        let kontakt2 = { Imie = "Jan"; Nazwisko = "Nowak"; DataUrodzenia = DateTime(1985,5,5); NrKartyBibliotecznej = "B234"; Email = None; Kontakt = AddressOnly "Warszawa, ul. Prosta 1" }
        let kontakt3 = { Imie = "Ewa"; Nazwisko = "Wiśniewska"; DataUrodzenia = DateTime(1995,7,7); NrKartyBibliotecznej = "C345"; Email = Some "ewa@example.com"; Kontakt = PhoneAndAddress("600333444", "Kraków, Rynek 2") }
        [
            { Id = "p1"; Opis = Some kontakt1; KaucjaUSD = 50.0M<USD>; DataDolaczenia = DateTime.UtcNow.AddDays(-400.); Status = StatusKonta.Standard; KaryPLN = 0.0M<PLN> }
            { Id = "p2"; Opis = Some kontakt2; KaucjaUSD = 30.0M<USD>; DataDolaczenia = DateTime.UtcNow.AddDays(-30.); Status = StatusKonta.Standard; KaryPLN = 10.0M<PLN> }
            { Id = "p3"; Opis = None; KaucjaUSD = 20.0M<USD>; DataDolaczenia = DateTime.UtcNow.AddDays(-800.); Status = StatusKonta.Premium; KaryPLN = 0.0M<PLN> }
            { Id = "p4"; Opis = Some kontakt3; KaucjaUSD = 100.0M<USD>; DataDolaczenia = DateTime.UtcNow.AddDays(-5.); Status = StatusKonta.Standard; KaryPLN = 0.0M<PLN> }
        ]

    let private sampleLoans : LoanHistory list =
        [
            { PatronId = "p1"; Loans = [ { ISBN = "978-83-01"; Returned = true }; { ISBN = "978-83-02"; Returned = false } ] }
            { PatronId = "p2"; Loans = [ { ISBN = "978-83-03"; Returned = true } ] }
            { PatronId = "p3"; Loans = [ { ISBN = "978-83-04"; Returned = true }; { ISBN = "978-83-05"; Returned = true }; { ISBN = "978-83-06"; Returned = true } ] }
            { PatronId = "p4"; Loans = [] }
        ]

    let writeSamplesIfMissing () =
        Files.ensureDataDirs ()
        if not (File.Exists Files.patronsFile) then
            let json = Serialization.serialize samplePatrons
            File.WriteAllText(Files.patronsFile, json)
        for lh in sampleLoans do
            let path = Files.loanFile lh.PatronId
            if not (File.Exists path) then
                let json = Serialization.serialize lh
                File.WriteAllText(path, json)

module Repo =
    let loadPatrons () : Czytelnik list =
        Files.ensureDataDirs ()
        if File.Exists Files.patronsFile then
            let json = File.ReadAllText Files.patronsFile
            Serialization.deserialize json |> Option.defaultValue []
        else []

    let savePatrons (patrons: Czytelnik list) =
        Files.ensureDataDirs ()
        let json = Serialization.serialize patrons
        File.WriteAllText(Files.patronsFile, json)

    let getLoanHistory (patronId: string) : LoanHistory =
        Files.ensureDataDirs ()
        let path = Files.loanFile patronId
        if File.Exists path then
            let json = File.ReadAllText path
            Serialization.deserialize json |> Option.defaultValue { PatronId = patronId; Loans = [] }
        else { PatronId = patronId; Loans = [] }

    let saveLoanHistory (history: LoanHistory) : unit =
        Files.ensureDataDirs ()
        let path = Files.loanFile history.PatronId
        let json = Serialization.serialize history
        File.WriteAllText(path, json)

module Currency =
    // Simple HTTP client (singleton)
    let http = new HttpClient()

    type Rates = { PLNperEUR: decimal; PLNperUSD: decimal }

    let private tryParseDecimal (s:string) =
        match Decimal.TryParse(s, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture) with
        | true, v -> Some v
        | _ -> None

    // stooq provides CSV like: "2025-11-03,12:00,4.2335"
    let private getLastValueFromStooq (symbol:string) =
        // Use .csv historical last row by adding &i=d for daily? We'll fetch simple CSV page and grab last numeric
        task {
            try
                let! resp = http.GetStringAsync($"https://stooq.pl/q/l/?s={symbol}")
                // Find last decimal in response
                let parts = resp.Split([|'\n';',';';';'\t';' '|], StringSplitOptions.RemoveEmptyEntries)
                let decs = parts |> Array.choose tryParseDecimal
                return decs |> Array.tryLast
            with _ -> return None
        }

    let fetchRates () = task {
        // Symbols: eurpln (PLN per EUR) and usdpln (PLN per USD)
        let! eur = getLastValueFromStooq "eurpln"
        let! usd = getLastValueFromStooq "usdpln"
        match eur, usd with
        | Some e, Some u -> return Some { PLNperEUR = e; PLNperUSD = u }
        | _ -> return None
    }

    let private strip (x:decimal<'u>) : decimal = decimal x
    let private withPLN (v:decimal) : decimal<PLN> = LanguagePrimitives.DecimalWithMeasure v
    let private withEUR (v:decimal) : decimal<EUR> = LanguagePrimitives.DecimalWithMeasure v
    let private withUSD (v:decimal) : decimal<USD> = LanguagePrimitives.DecimalWithMeasure v

    let plnToEur (rates:Rates) (x: decimal<PLN>) : decimal<EUR> = strip x / rates.PLNperEUR |> withEUR
    let plnToUsd (rates:Rates) (x: decimal<PLN>) : decimal<USD> = strip x / rates.PLNperUSD |> withUSD
    let usdToPln (rates:Rates) (x: decimal<USD>) : decimal<PLN> = strip x * rates.PLNperUSD |> withPLN
    let eurToPln (rates:Rates) (x: decimal<EUR>) : decimal<PLN> = strip x * rates.PLNperEUR |> withPLN

module Domain =
    let ensureSampleData () = SampleData.writeSamplesIfMissing ()
    let loadPatrons () = Repo.loadPatrons ()

    let getLoanHistory (patronId: string) : Loan list =
        let h = Repo.getLoanHistory patronId
        h.Loans

    let isPatronForLongerThan (days:int) (patron: Czytelnik) : bool =
        (DateTime.UtcNow - patron.DataDolaczenia).TotalDays > float days

    let addFine (amount: decimal<PLN>) (patron: Czytelnik) : Czytelnik =
        { patron with KaryPLN = patron.KaryPLN + amount }

    type Library(patronsInit: Czytelnik list) =
        let mutable patrons = patronsInit

        member _.GetPatron(id:string) = patrons |> List.tryFind (fun p -> p.Id = id)
        member _.GetAll() = patrons

        member _.Save() = Repo.savePatrons patrons

        member this.AddFine(id:string, amount:decimal<PLN>) =
            patrons <- patrons |> List.map (fun p -> if p.Id = id then addFine amount p else p)
            this.Save()

        member _.PromoteIfEligible(id:string, minLoans:int, minDays:int) =
            match patrons |> List.tryFind (fun p -> p.Id = id) with
            | None -> false
            | Some p ->
                let loans = getLoanHistory id |> List.length
                let longEnough = isPatronForLongerThan minDays p
                if p.Status = StatusKonta.Standard && loans > minLoans && longEnough then
                    patrons <- patrons |> List.map (fun x -> if x.Id = id then { x with Status = StatusKonta.Premium } else x)
                    true
                else false

        member _.TotalFinesPLN(id:string) =
            patrons |> List.tryFind (fun p -> p.Id = id) |> Option.map (fun p -> p.KaryPLN)

        member _.DepositUSD(id:string) =
            patrons |> List.tryFind (fun p -> p.Id = id) |> Option.map (fun p -> p.KaucjaUSD)

        member this.FinesExceedDeposit(id:string, rates: Currency.Rates) =
            match this.TotalFinesPLN id, this.DepositUSD id with
            | Some fines, Some deposit ->
                let depPln = Currency.usdToPln rates deposit
                fines > depPln
            | _ -> false

        // New functions to modify data via TUI
        member this.AddOrReplacePatron(p: Czytelnik) =
            let exists = patrons |> List.exists (fun x -> x.Id = p.Id)
            patrons <-
                if exists then patrons |> List.map (fun x -> if x.Id = p.Id then p else x)
                else p :: patrons
            this.Save()

        member _.GetLoanHistory(id:string) : Loan list =
            Repo.getLoanHistory id |> fun h -> h.Loans

        member _.AddLoan(id:string, isbn:string) =
            let h = Repo.getLoanHistory id
            let updated = { h with Loans = h.Loans @ [ { ISBN = isbn; Returned = false } ] }
            Repo.saveLoanHistory updated

        member _.MarkLoanReturned(id:string, isbn:string) =
            let h = Repo.getLoanHistory id
            let updatedLoans =
                let mutable marked = false
                h.Loans |> List.map (fun l ->
                    if not marked && l.ISBN = isbn && l.Returned = false then
                        marked <- true; { l with Returned = true }
                    else l)
            let updated = { h with Loans = updatedLoans }
            Repo.saveLoanHistory updated
