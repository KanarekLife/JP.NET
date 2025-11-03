open System
open LibraryApp

let readLine prompt =
    printf "%s" prompt
    Console.ReadLine()

let readOptional prompt =
    let s = readLine (prompt + " (ENTER = brak): ")
    if String.IsNullOrWhiteSpace s then None else Some s

let readDate prompt =
    let s = readLine (prompt + " (rrrr-mm-dd, ENTER = dziś): ")
    if String.IsNullOrWhiteSpace s then DateTime.UtcNow
    else match DateTime.TryParse s with | true, d -> d | _ -> printfn "Błędna data. Ustawiono dziś."; DateTime.UtcNow

let readDecimal prompt =
    let s = readLine prompt
    match Decimal.TryParse s with | true, v -> Some v | _ -> printfn "Błędna liczba."; None

let printPatronBasic (p: Czytelnik) =
    match p.Opis with
    | Some d -> printfn "[%s] %s %s | Email: %s | Status: %A | Kary: %M PLN | Kaucja: %M USD" p.Id d.Imie d.Nazwisko (defaultArg d.Email "brak") p.Status (decimal p.KaryPLN) (decimal p.KaucjaUSD)
    | None -> printfn "[%s] (brak danych opisowych) | Status: %A | Kary: %M PLN | Kaucja: %M USD" p.Id p.Status (decimal p.KaryPLN) (decimal p.KaucjaUSD)

let printMenu () =
    printfn "\n=== Biblioteka (TUI) ==="
    printfn "0. Pokaż kursy walut i odśwież"
    printfn "1. Lista czytelników"
    printfn "2. Pokaż informacje o czytelniku (Imię, Nazwisko, Email)"
    printfn "3. Dodaj karę (PLN) do konta czytelnika"
    printfn "4. Sprawdź czy suma kar (PLN) przekracza kaucję (USD)"
    printfn "5. Awansuj czytelnika (na podstawie historii i stażu)"
    printfn "6. Dodaj nowego/aktualizuj czytelnika"
    printfn "7. Dodaj wypożyczenie (ISBN)"
    printfn "8. Oznacz wypożyczenie jako zwrócone"
    printfn "9. Zapisz i wyjdź"

[<EntryPoint>]
let main _ =
    Domain.ensureSampleData ()
    let patrons = Domain.loadPatrons ()
    let lib = Domain.Library patrons

    let mutable ratesOpt = Currency.fetchRates().Result
    match ratesOpt with
    | None -> printfn "Nie udało się pobrać kursów walut ze stooq.pl."
    | Some r -> printfn "Pobrano kursy: 1 EUR = %M PLN, 1 USD = %M PLN" r.PLNperEUR r.PLNperUSD

    let rec loop () =
        printMenu()
        match readLine "> " with
        | "0" ->
            ratesOpt <- Currency.fetchRates().Result
            match ratesOpt with
            | Some r -> printfn "Aktualne kursy: 1 EUR = %M PLN, 1 USD = %M PLN" r.PLNperEUR r.PLNperUSD
            | None -> printfn "Brak kursów (błąd pobierania)."
            loop()
        | "1" ->
            lib.GetAll() |> List.sortBy (fun p -> p.Id) |> List.iter printPatronBasic
            loop()
        | "2" ->
            let id = readLine "Podaj ID czytelnika: "
            match lib.GetPatron id with
            | Some p ->
                printPatronBasic p
                // Pokaż również historię wypożyczeń
                let loans = lib.GetLoanHistory id
                if loans.IsEmpty then printfn "Brak wypożyczeń."
                else loans |> List.iter (fun l -> printfn "- ISBN: %s | Zwrócona: %b" l.ISBN l.Returned)
            | None -> printfn "Nie znaleziono"
            loop()
        | "3" ->
            let id = readLine "Podaj ID czytelnika: "
            match readDecimal "Podaj kwotę w PLN: " with
            | Some v -> lib.AddFine(id, (v * 1.0M<PLN>)); printfn "Dodano karę."
            | None -> ()
            loop()
        | "4" ->
            let id = readLine "Podaj ID czytelnika: "
            match ratesOpt with
            | Some r ->
                let exceeds = lib.FinesExceedDeposit(id, r)
                printfn (if exceeds then "Suma kar przekracza kaucję." else "Suma kar nie przekracza kaucji.")
            | None -> printfn "Brak kursów walut. Użyj opcji 0, aby odświeżyć."
            loop()
        | "5" ->
            let id = readLine "Podaj ID czytelnika: "
            let loansStr = readLine "Minimalna liczba wypożyczeń: "
            let daysStr = readLine "Minimalna liczba dni w bibliotece: "
            match Int32.TryParse loansStr, Int32.TryParse daysStr with
            | (true, l), (true, d) ->
                let promoted = lib.PromoteIfEligible(id, l, d)
                printfn (if promoted then "Awansowano na Premium" else "Brak spełnionych warunków")
            | _ -> printfn "Błędne wartości"
            loop()
        | "6" ->
            let id = readLine "ID czytelnika: "
            let kaucja = readDecimal "Kaucja (USD): "
            let dataDol = readDate "Data dołączenia"
            let status = readLine "Status (Standard/Premium): "
            let statusParsed = if status.Trim().ToLower() = "premium" then StatusKonta.Premium else StatusKonta.Standard
            // Dane kontaktowe opcjonalne
            let hasOpis = readLine "Dodać dane opisowe? (t/n): "
            let opisOpt =
                if hasOpis.Trim().ToLower() = "t" then
                    let imie = readLine "Imię: "
                    let nazw = readLine "Nazwisko: "
                    let ur = readDate "Data urodzenia"
                    let karta = readLine "Nr karty bibliotecznej: "
                    let email = readOptional "Email"
                    let phone = readOptional "Telefon"
                    let addr = readOptional "Adres pocztowy"
                    let kontakt =
                        match phone, addr with
                        | Some ph, Some ad -> ContactInfo.PhoneAndAddress(ph, ad)
                        | Some ph, None -> ContactInfo.PhoneOnly ph
                        | None, Some ad -> ContactInfo.AddressOnly ad
                        | None, None ->
                            printfn "Wymagany telefon lub adres. Anulowano dane opisowe."; ContactInfo.PhoneOnly "brak"
                    Some { Imie = imie; Nazwisko = nazw; DataUrodzenia = ur; NrKartyBibliotecznej = karta; Email = email; Kontakt = kontakt }
                else None
            match kaucja with
            | Some k ->
                let nowy: Czytelnik = { Id = id; Opis = opisOpt; KaucjaUSD = (k * 1.0M<USD>); DataDolaczenia = dataDol; Status = statusParsed; KaryPLN = 0.0M<PLN> }
                lib.AddOrReplacePatron nowy
                printfn "Zapisano czytelnika."
            | None -> ()
            loop()
        | "7" ->
            let id = readLine "ID czytelnika: "
            let isbn = readLine "ISBN książki: "
            lib.AddLoan(id, isbn)
            printfn "Dodano wypożyczenie."
            loop()
        | "8" ->
            let id = readLine "ID czytelnika: "
            let isbn = readLine "ISBN książki do oznaczenia jako zwrócona: "
            lib.MarkLoanReturned(id, isbn)
            printfn "Zaktualizowano wypożyczenie."
            loop()
        | "9" ->
            lib.Save()
            0
        | _ ->
            printfn "Nieznana opcja. Wybierz 0-9."
            loop()
    loop()
