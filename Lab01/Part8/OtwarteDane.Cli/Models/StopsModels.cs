using Newtonsoft.Json;

namespace OtwarteDane.Cli.Models;

public class StopsData
{
    [JsonProperty("lastUpdate")]
    public DateTime LastUpdate { get; set; }

    [JsonProperty("stops")]
    public List<Stop> Stops { get; set; } = new();
}

public class Stop
{
    [JsonProperty("stopId")]
    public int StopId { get; set; }

    [JsonProperty("stopCode")]
    public string? StopCode { get; set; }

    [JsonProperty("stopName")]
    public string? StopName { get; set; }

    [JsonProperty("stopShortname")]
    public int StopShortname { get; set; }

    [JsonProperty("stopDesc")]
    public string StopDesc { get; set; } = string.Empty;

    [JsonProperty("subName")]
    public string? SubName { get; set; }

    [JsonProperty("date")]
    public string Date { get; set; } = string.Empty;

    [JsonProperty("stopLat")]
    public double StopLat { get; set; }

    [JsonProperty("stopLon")]
    public double StopLon { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("zoneId")]
    public int? ZoneId { get; set; }

    [JsonProperty("zoneName")]
    public string ZoneName { get; set; } = string.Empty;

    [JsonProperty("wheelchairBoarding")]
    public int? WheelchairBoarding { get; set; }

    [JsonProperty("virtual")]
    public int? Virtual { get; set; }

    [JsonProperty("nonpassenger")]
    public int? Nonpassenger { get; set; }

    [JsonProperty("depot")]
    public int? Depot { get; set; }

    [JsonProperty("ticketZoneBorder")]
    public int? TicketZoneBorder { get; set; }

    [JsonProperty("onDemand")]
    public int? OnDemand { get; set; }

    [JsonProperty("activationDate")]
    public string ActivationDate { get; set; } = string.Empty;
}