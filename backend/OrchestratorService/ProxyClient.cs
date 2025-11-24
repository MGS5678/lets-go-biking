using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;


public class ProxyClient
{
    private readonly HttpClient _httpClient;
    private readonly string baseUrl;
    public ProxyClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        baseUrl = "http://localhost:8733/Design_Time_Addresses/Proxy/ProxyService/";
    }

    public async Task<List<Station>> GetStations(string contractName)
    {
        Debug.WriteLine("ProxyClient.cs");
        var requestUrl = $"{baseUrl}/stations?contract={contractName}";
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var stationsJson = await response.Content.ReadAsStringAsync();
        if (stationsJson != null)
        {
            string actualJson = JsonConvert.DeserializeObject<string>(stationsJson);
            List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(actualJson);
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
        var requestUrl = $"{baseUrl}/contractName?city={city}";
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var contractNameRaw = await response.Content.ReadAsStringAsync();

        string contractName;
        try
        {
            contractName = JsonConvert.DeserializeObject<string>(contractNameRaw);
            Debug.WriteLine("ProxyClient.cs - GetContractNameFromCity - returned contract: " + contractName + " for city: " + city);
        }
        catch (JsonException)
        {
            contractName = contractNameRaw.Trim('"');
            Debug.WriteLine("ProxyClient.cs - GetContractNameFromCity - (fallback) returned contract: " + contractName + " for city: " + city);
        }

        return contractName;
    }

    public async Task<string> GetCoords(string address)
    {
        var requestUrl = $"{baseUrl}/coords?address={Uri.EscapeDataString(address)}";
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var coordsJson = await response.Content.ReadAsStringAsync();
        string actualJson = JsonConvert.DeserializeObject<string>(coordsJson);
        return actualJson;
    }

    public async Task<string> GetRoute(string coords1, string coords2, string meansTransport)
    {
        Debug.WriteLine("ProxyClient.cs - GetRoute called");
        var requestUrl = $"{baseUrl}/route?coords1={Uri.EscapeDataString(coords1)}&coords2={Uri.EscapeDataString(coords2)}&meansTransport={Uri.EscapeDataString(meansTransport)}";
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var routeJson = await response.Content.ReadAsStringAsync();
        string actualJson = JsonConvert.DeserializeObject<string>(routeJson);
        Debug.WriteLine("ProxyClient.cs - GetRoute - returned route for transport: " + meansTransport);
        return actualJson;
    }

    public async Task<string> GetAllStations()
    {
        var requestUrl = $"{baseUrl}/allstations";
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var allStationsJson = await response.Content.ReadAsStringAsync();
        string actualJson = JsonConvert.DeserializeObject<string>(allStationsJson);
        return actualJson;
    }
}