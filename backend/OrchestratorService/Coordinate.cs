using Newtonsoft.Json;
using System;

public class Coordinate
{
    [JsonProperty("latitude")]
    private double lat { get; set; }

    [JsonProperty("longitude")]
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
    public static double Distance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Rayon de la Terre en km

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var rlat1 = ToRadians(lat1);
        var rlat2 = ToRadians(lat2);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(rlat1) * Math.Cos(rlat2)
            * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private static double ToRadians(double deg)
    {
        return deg * (Math.PI / 180);
    }
    public double DistanceTo(Coordinate other)
    {
        return Distance(lat, lng, other.lat, other.lng);
    }
    public double DistanceTo(double otherLat, double otherLng)
    {
        return Distance(lat, lng, otherLat, otherLng);
    }
    override
    public string ToString()
    {
        return "[" + lng.ToString().Replace(',', '.') + "," + lat.ToString().Replace(',', '.') + "]";
    }
}