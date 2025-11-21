using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrchestratorService.ProxyService;


public class ProxyClient
{
    HttpClient _httpClient;
    public ProxyClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Station>> getStations(string contractName)
    {
        var proxyService = new ProxyServiceClient();
        string stationsJson = await proxyService.GetStationsJsonAsync(contractName);
        if (stationsJson!=null)
        {
            List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(stationsJson);
            return stations;
        }
        else
        {
            Console.WriteLine("StationsJson was null");
            return null;
        }
    }
}