using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class OpenRouteClient
    {
        HttpClient _httpClient;
        string apiKey = "eyJvcmciOiI1YjNjZTM1OTc4NTExMTAwMDFjZjYyNDgiLCJpZCI6ImQ0MWYyZTg4N2Q4MDQwNWU5ZWQxY2U2MmRhOTNkNGNkIiwiaCI6Im11cm11cjY0In0=";
        string url = "https://api.openrouteservice.org/geocode/";
        public OpenRouteClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetCoordinates(string address) // recup les coords en json
        {
            string cleanAddress = address?.Trim('"') ?? address;
            string requestUrl = $"{url}search?api_key={apiKey}&text={Uri.EscapeDataString(cleanAddress)}";

            Debug.WriteLine("OpenRouteClient.cs");
            Debug.WriteLine(requestUrl);
            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("succes de la requete de coords");
                string responseMessage = await response.Content.ReadAsStringAsync();

                JObject responseJson = JObject.Parse(responseMessage);
                Debug.WriteLine("B");

                JArray features = (JArray)responseJson["features"];

                if (features != null && features.Count > 0)
                {
                    JObject firstFeature = (JObject)features[0];
                    JArray coords = (JArray)firstFeature["geometry"]["coordinates"];

                    string coordsJsonString = coords.ToString(Newtonsoft.Json.Formatting.None);
                    Debug.WriteLine("OpenRouteClient.cs - GetCoordinates - returned coordinates for address: " + cleanAddress);
                    Debug.WriteLine("Coordinates JSON: " + coordsJsonString);
                    return coordsJsonString;
                }
                else
                {
                    Debug.WriteLine("Aucune coordonnée trouvée");
                    return null;
                }
            }
            else
            {
                Debug.WriteLine("echec de la requete de coords");
                Console.WriteLine("GetCoordinates request failed " + response.StatusCode + " - " + response.ReasonPhrase);
                return null;
            }
        }
    }
}
