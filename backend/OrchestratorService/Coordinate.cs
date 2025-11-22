using Newtonsoft.Json;
using System;

public class Coordinate
{
    [JsonProperty("lat")]
    private double lat { get; set; }

    [JsonProperty("lng")]
    private double lng { get; set; }

    public Coordinate()
    {
        // empty for json
    }

    public Coordinate(double lat, double lng)
    {
        this.lat = lat;
        this.lng = lng;
    }

    public double distanceTo(Coordinate other)
    {
        return Math.Sqrt(Math.Pow(other.lat - lat, 2) + Math.Pow(other.lng - lng, 2));
    }
}