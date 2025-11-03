namespace LibraryApp

open System

[<Measure>] type PLN
[<Measure>] type EUR
[<Measure>] type USD

type ContactInfo =
    | PhoneOnly of phone:string
    | AddressOnly of address:string
    | PhoneAndAddress of phone:string * address:string

type DaneKontaktowe = {
    Imie: string
    Nazwisko: string
    DataUrodzenia: DateTime
    NrKartyBibliotecznej: string
    Email: string option
    Kontakt: ContactInfo
}

type StatusKonta =
    | Standard
    | Premium

type Czytelnik = {
    Id: string
    Opis: DaneKontaktowe option
    KaucjaUSD: decimal<USD>
    DataDolaczenia: DateTime
    Status: StatusKonta
    KaryPLN: decimal<PLN>
}

type Loan = { ISBN: string; Returned: bool }

type LoanHistory = { PatronId: string; Loans: Loan list }

module Currency =
    type Rates = { PLNperEUR: decimal; PLNperUSD: decimal }
    val fetchRates: unit -> System.Threading.Tasks.Task<Rates option>
    val plnToEur: rates:Rates -> x:decimal<PLN> -> decimal<EUR>
    val plnToUsd: rates:Rates -> x:decimal<PLN> -> decimal<USD>
    val usdToPln: rates:Rates -> x:decimal<USD> -> decimal<PLN>
    val eurToPln: rates:Rates -> x:decimal<EUR> -> decimal<PLN>

module Domain =
    val ensureSampleData: unit -> unit
    val loadPatrons: unit -> Czytelnik list
    val getLoanHistory: patronId:string -> Loan list
    val isPatronForLongerThan: days:int -> patron:Czytelnik -> bool
    val addFine: amount:decimal<PLN> -> patron:Czytelnik -> Czytelnik

    type Library =
        new: patronsInit: Czytelnik list -> Library
        member GetPatron: id:string -> Czytelnik option
        member GetAll: unit -> Czytelnik list
        member Save: unit -> unit
        member AddFine: id:string * amount:decimal<PLN> -> unit
        member PromoteIfEligible: id:string * minLoans:int * minDays:int -> bool
        member TotalFinesPLN: id:string -> decimal<PLN> option
        member DepositUSD: id:string -> decimal<USD> option
        member FinesExceedDeposit: id:string * rates: Currency.Rates -> bool
        member AddOrReplacePatron: p:Czytelnik -> unit
        member GetLoanHistory: id:string -> Loan list
        member AddLoan: id:string * isbn:string -> unit
        member MarkLoanReturned: id:string * isbn:string -> unit
