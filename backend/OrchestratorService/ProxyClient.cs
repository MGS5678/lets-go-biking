using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrchestratorService.ProxyService;


public class ProxyClient
{
    private readonly HttpClient _httpClient;
    private readonly ProxyServiceClient proxyService;
    public ProxyClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        proxyService = new ProxyServiceClient();
    }

    public async Task<List<Station>> GetStations(string contractName)
    {
        Debug.WriteLine("ProxyClient.cs");
        string stationsJson = await proxyService.GetStationsJsonAsync(contractName);
        if (stationsJson != null)
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

    public async Task<string> GetContractNameFromCity(string city)
    {
        string contractName = await proxyService.GetContractNameFromCityAsync(city);
        Debug.WriteLine("ProxyClient.cs - GetContractNameFromCity - returned contract: " + contractName + " for city: " + city);
        return contractName;
    }
}