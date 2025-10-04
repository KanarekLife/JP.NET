using Newtonsoft.Json;
using OtwarteDane.Cli.Models;

namespace OtwarteDane.Cli.Services;

public class ZtmApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://ckan2.multimediagdansk.pl";
    private readonly string _stopsUrl = "https://ckan.multimediagdansk.pl/dataset/c24aa637-3619-4dc2-a171-a23eec8f2172/resource/4c4025f0-01bf-41f7-a39f-d156d201b82b/download/stops.json";

    public ZtmApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "OtwarteDane.Cli/1.0");
    }

    public async Task<DeparturesResponse?> GetDeparturesAsync(int stopId)
    {
        try
        {
            var url = $"{_baseUrl}/departures?stopId={stopId}";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Błąd HTTP: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DeparturesResponse>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas pobierania danych: {ex.Message}");
            return null;
        }
    }

    public async Task<Dictionary<string, StopsData>?> GetStopsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(_stopsUrl);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Błąd HTTP: {response.StatusCode}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, StopsData>>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas pobierania listy przystanków: {ex.Message}");
            return null;
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}