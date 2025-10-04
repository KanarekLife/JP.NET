using System.CommandLine;
using OtwarteDane.Cli.Services;
using OtwarteDane.Cli.Models;

var rootCommand = new RootCommand("Narzędzie CLI do otwartych danych ZTM w Gdańsku");

var stopIdArgument = new Argument<int>("stopId", "ID przystanku (np. 1001)");
var departuresCommand = new Command("departures", "Pobiera estymowane czasy odjazdów z przystanku");
departuresCommand.AddArgument(stopIdArgument);
departuresCommand.SetHandler(async (int stopId) =>
{
    var apiService = new ZtmApiService();
    
    Console.WriteLine($"Pobieranie odjazdów dla przystanku ID: {stopId}...\n");
    
    var departures = await apiService.GetDeparturesAsync(stopId);
    
    if (departures == null)
    {
        Console.WriteLine("Nie udało się pobrać danych o odjazdach.");
        return;
    }

    Console.WriteLine($"Ostatnia aktualizacja: {departures.LastUpdate:yyyy-MM-dd HH:mm:ss}");
    Console.WriteLine($"Liczba odjazdów: {departures.Departures.Count}\n");

    if (departures.Departures.Count == 0)
    {
        Console.WriteLine("Brak zaplanowanych odjazdów dla tego przystanku.");
        return;
    }

    Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║ Linia │   Kierunek            │ Rozkład  │ Estymowany │ Opóźnienie │ Status   ║");
    Console.WriteLine("╠═══════════════════════════════════════════════════════════════════════════════╣");
    
    foreach (var departure in departures.Departures.Take(15)) // Pokazuje pierwsze 15 odjazdów
    {
        var delay = departure.DelayInSeconds.HasValue 
            ? $"{departure.DelayInSeconds.Value / 60:+#;-#;0} min" 
            : "---";
        
        var status = departure.Status == "REALTIME" ? "Na żywo" : "Rozkład";
        var statusIcon = departure.Status == "REALTIME" ? "🔴" : "🕐";
        
        var headsign = departure.Headsign.Length > 15 
            ? departure.Headsign.Substring(0, 12) + "..." 
            : departure.Headsign;

        Console.WriteLine($"║ {departure.RouteShortName.PadRight(5)} │ {headsign.PadRight(17)} │ {departure.TheoreticalTime:HH:mm} │ {departure.EstimatedTime:HH:mm}     │ {delay.PadLeft(10)} │ {statusIcon} {status.PadRight(6)} ║");
    }
    
    Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════════╝");
    
    if (departures.Departures.Count > 15)
    {
        Console.WriteLine($"\n... i {departures.Departures.Count - 15} więcej odjazdów.");
    }
    
    apiService.Dispose();
}, stopIdArgument);

var queryArgument = new Argument<string>("query", "Nazwa przystanku do wyszukania");
var searchCommand = new Command("search", "Wyszukuje przystanki po nazwie");
searchCommand.AddArgument(queryArgument);
searchCommand.SetHandler(async (string query) =>
{
    var apiService = new ZtmApiService();
    
    Console.WriteLine($"Wyszukiwanie przystanków zawierających '{query}'...\n");
    
    var stopsData = await apiService.GetStopsAsync();
    
    if (stopsData == null)
    {
        Console.WriteLine("Nie udało się pobrać listy przystanków.");
        return;
    }

    var allStops = new List<Stop>();
    foreach (var dateData in stopsData.Values)
    {
        allStops.AddRange(dateData.Stops);
    }

    var matchingStops = allStops
        .Where(s => s.StopDesc.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                   (s.StopName?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
        .GroupBy(s => s.StopDesc)
        .Select(g => g.First())
        .Take(20)
        .OrderBy(s => s.StopDesc)
        .ToList();

    if (matchingStops.Count == 0)
    {
        Console.WriteLine($"Nie znaleziono przystanków zawierających '{query}'.");
        return;
    }

    Console.WriteLine($"Znaleziono {matchingStops.Count} przystanków:\n");
    Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║ ID     │ Nazwa przystanku                               │ Typ         ║");
    Console.WriteLine("╠═══════════════════════════════════════════════════════════════════════╣");
    
    foreach (var stop in matchingStops)
    {
        var stopName = stop.StopDesc.Length > 45 
            ? stop.StopDesc.Substring(0, 42) + "..." 
            : stop.StopDesc;
        
        var typeIcon = stop.Type switch
        {
            "BUS" => "🚌",
            "TRAM" => "🚋",
            "BUS_TRAM" => "🚌🚋",
            _ => "❓"
        };
        
        Console.WriteLine($"║ {stop.StopId.ToString().PadRight(6)} │ {stopName.PadRight(46)} │ {typeIcon} {stop.Type.PadRight(8)} ║");
    }
    
    Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════╝");
    Console.WriteLine("\nAby sprawdzić odjazdy z przystanku, użyj komendy:");
    Console.WriteLine("dotnet run departures <ID>");
    
    apiService.Dispose();
}, queryArgument);

var infoStopIdArgument = new Argument<int>("stopId", "ID przystanku");
var infoCommand = new Command("info", "Wyświetla szczegółowe informacje o przystanku");
infoCommand.AddArgument(infoStopIdArgument);
infoCommand.SetHandler(async (int stopId) =>
{
    var apiService = new ZtmApiService();
    
    Console.WriteLine($"Pobieranie informacji o przystanku ID: {stopId}...\n");
    
    var stopsData = await apiService.GetStopsAsync();
    
    if (stopsData == null)
    {
        Console.WriteLine("Nie udało się pobrać listy przystanków.");
        return;
    }

    Stop? foundStop = null;
    foreach (var dateData in stopsData.Values)
    {
        foundStop = dateData.Stops.FirstOrDefault(s => s.StopId == stopId);
        if (foundStop != null) break;
    }

    if (foundStop == null)
    {
        Console.WriteLine($"Nie znaleziono przystanku o ID: {stopId}");
        return;
    }

    Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                           INFORMACJE O PRZYSTANKU                            ║");
    Console.WriteLine("╠═══════════════════════════════════════════════════════════════════════════════╣");
    Console.WriteLine($"║ ID przystanku:      {foundStop.StopId.ToString().PadRight(58)} ║");
    Console.WriteLine($"║ Nazwa:              {foundStop.StopDesc.PadRight(58)} ║");
    if (!string.IsNullOrEmpty(foundStop.StopName))
        Console.WriteLine($"║ Nazwa pełna:        {foundStop.StopName.PadRight(58)} ║");
    if (!string.IsNullOrEmpty(foundStop.SubName))
        Console.WriteLine($"║ Numer słupka:       {foundStop.SubName.PadRight(58)} ║");
    Console.WriteLine($"║ Typ:                {GetTypeDescription(foundStop.Type).PadRight(58)} ║");
    Console.WriteLine($"║ Strefa:             {foundStop.ZoneName.PadRight(58)} ║");
    Console.WriteLine($"║ Współrzędne:        {foundStop.StopLat:F6}, {foundStop.StopLon:F6}".PadRight(78) + " ║");
    Console.WriteLine($"║ Dostępność dla      {((foundStop.WheelchairBoarding ?? 0) == 1 ? "TAK" : "NIE").PadRight(58)} ║");
    Console.WriteLine($"║ wózków inwalidzkich:                                                          ║");
    Console.WriteLine($"║ Na żądanie:         {((foundStop.OnDemand ?? 0) == 1 ? "TAK" : "NIE").PadRight(58)} ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════════╝");
    
    apiService.Dispose();
}, infoStopIdArgument);

static string GetTypeDescription(string type)
{
    return type switch
    {
        "BUS" => "🚌 Autobus",
        "TRAM" => "🚋 Tramwaj", 
        "BUS_TRAM" => "🚌🚋 Autobus i Tramwaj",
        _ => "❓ Nieznany"
    };
}

rootCommand.AddCommand(departuresCommand);
rootCommand.AddCommand(searchCommand);
rootCommand.AddCommand(infoCommand);

return await rootCommand.InvokeAsync(args);
