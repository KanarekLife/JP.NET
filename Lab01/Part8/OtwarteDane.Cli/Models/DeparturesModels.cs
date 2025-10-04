using Newtonsoft.Json;

namespace OtwarteDane.Cli.Models;

public class DeparturesResponse
{
    [JsonProperty("lastUpdate")]
    public DateTime LastUpdate { get; set; }

    [JsonProperty("departures")]
    public List<Departure> Departures { get; set; } = new();
}

public class Departure
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("delayInSeconds")]
    public int? DelayInSeconds { get; set; }

    [JsonProperty("estimatedTime")]
    public DateTime EstimatedTime { get; set; }

    [JsonProperty("headsign")]
    public string Headsign { get; set; } = string.Empty;

    [JsonProperty("routeShortName")]
    public string RouteShortName { get; set; } = string.Empty;

    [JsonProperty("routeId")]
    public int RouteId { get; set; }

    [JsonProperty("scheduledTripStartTime")]
    public DateTime ScheduledTripStartTime { get; set; }

    [JsonProperty("tripId")]
    public int TripId { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("theoreticalTime")]
    public DateTime TheoreticalTime { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonProperty("trip")]
    public int Trip { get; set; }

    [JsonProperty("vehicleCode")]
    public int? VehicleCode { get; set; }

    [JsonProperty("vehicleId")]
    public int? VehicleId { get; set; }

    [JsonProperty("vehicleService")]
    public string? VehicleService { get; set; }
}