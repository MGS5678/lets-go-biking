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
        Debug.WriteLine("Route JSON: " + actualJson);
        return actualJson;
    }

    public async Task<string> GetAllStations()
    {
        var requestUrl = $"{baseUrl}/allstations";
        var response = await _httpClient.GetAsync(requestUrl);
        response.EnsureSuccessStatusCode();
        var allStationsJson = await response.Content.ReadAsStringAsync();
        string actualJson = JsonConvert.DeserializeObject<string>(allStationsJson);
        Debug.WriteLine("ProxyClient.cs - GetAllStations - returned all stations");
        return actualJson;
    }
}