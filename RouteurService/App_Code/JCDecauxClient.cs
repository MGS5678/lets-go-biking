using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class JCDecauxClient
{
    HttpClient _httpClient;
    string apiKey = "668bf3fd91bde76a99077bb91ec0c543339a25b2";
    string url = "https://api.jcdecaux.com/vls/v1/";
    public JCDecauxClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Station>> getStations(string contractName)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(url + "/stations?contract=" + contractName + "&apiKey=" + apiKey);
        
        if (response.IsSuccessStatusCode)
        {
            string responseMessage = await response.Content.ReadAsStringAsync();
            List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(responseMessage);
            return stations;
        }
        else
        {
            Console.WriteLine("Request failed " + response.StatusCode + " - " + response.ReasonPhrase);
            return null;
        }
    }
}