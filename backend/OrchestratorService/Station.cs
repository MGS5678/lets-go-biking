using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;

public class Station
{
    [JsonProperty("number")]
    public int number { get; set; }
    [JsonProperty("contract_name")]
    public string contract_name { get; set; }
    [JsonProperty("name")]
    public string name { get; set; }
    [JsonProperty("address")]
    public string address { get; set; }
    [JsonProperty("position")]
    public Coordinate position { get; set; }
    [JsonProperty("banking")]
    public bool banking { get; set; }
    [JsonProperty("bonus")]
    public bool bonus { get; set; }
    [JsonProperty("bike_stands")]
    public int bike_stands { get; set; }
    [JsonProperty("available_bike_stands")]
    public int available_bike_stands { get; set; }
    [JsonProperty("available_bikes")]
    public int available_bikes { get; set; }
    [JsonProperty("status")]
    public string status { get; set; }
    [JsonProperty("last_update")]
    public long? last_update { get; set; }


    public Station()
    {
        // empty for json
    }
}