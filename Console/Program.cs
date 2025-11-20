using RouteurServiceReference;
using System;
using System.Data;
using System.Text.Json;

namespace ConsoleApp
{
    internal class ConsoleApp
    {
        static void Main(string[] args)
        {
            CallServiceAsync().GetAwaiter().GetResult();
        }

        static async Task CallServiceAsync()
        {
            HttpClient client = new HttpClient();
            Coordinate from = new Coordinate(50.806801, 4.3184);  // near 231 - KERSBEEK
            Coordinate to = new Coordinate(50.822567, 4.357508);  // near 112 - PAGE / EDELKNAAP

            string args = $"fromlat={from.lat}&fromlng={from.lng}&tolat={to.lat}&tolng={to.lng}";
            string url = $"http://localhost:55111/RouteurService.svc/Get?{args}";
            url = url.Replace(",", ".");  // from.lat.ToString() return a double with a comma instead of a . (because of the culture format)

            string result = await client.GetStringAsync(url);
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(result);
            string prettyJson = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(prettyJson);
        }
    }

    public class Coordinate
    {
        public Coordinate(double lat, double lng)
        { 
            this.lat = lat;
            this.lng = lng;
        }

        public double lat { get; set; }

        public double lng { get; set; }
    }
}