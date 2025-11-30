using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Proxy.valueobjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Net.WebRequestMethods;

namespace Proxy
{

    public class OpenMeteoClient
    {
        HttpClient _httpClient;
        string url = "https://api.open-meteo.com/v1/";
        public OpenMeteoClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<Meteo> GetMeteoFromCoords(string coords)
        {
            Debug.WriteLine(coords);
            coords = coords.Trim('"');
            coords = coords.Trim('[', ']');
            string latitude = coords.Split(',')[0];
            string longitude = coords.Split(',')[1];
            string requestUrl = url + "forecast?latitude=" + latitude + "&longitude=" + longitude + "&current_weather=true";
            Debug.WriteLine("OpenMeteoClient.cs");
            Debug.WriteLine(requestUrl);
            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseMessage = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("OpenMeteoClient.cs - returned meteo");
                return JsonConvert.DeserializeObject<Meteo>(responseMessage);
            }
            else
            {
                Console.WriteLine("Meteo request failed" + response.StatusCode + " - " + response.ReasonPhrase);
                return null;
            }
        }
    }
}