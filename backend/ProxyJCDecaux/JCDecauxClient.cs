using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProxyJCDecaux
{

    public class JCDecauxClient
    {
        HttpClient _httpClient;
        string apiKey = "668bf3fd91bde76a99077bb91ec0c543339a25b2";
        string url = "https://api.jcdecaux.com/vls/v1/";
        public JCDecauxClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, List<string>>> GetContracts() // r�cup�re les contrats et les met dans un dico
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url + "contracts?apiKey=" + apiKey);

            if (response.IsSuccessStatusCode)
            {
                string responseMessage = await response.Content.ReadAsStringAsync();
                JArray parsedData = JArray.Parse(responseMessage);
                Dictionary<string, List<string>> contracts = new Dictionary<string, List<string>>();
                foreach (var item in parsedData)
                {
                    string contractName = item["name"].ToString();
                    List<string> cities = item["cities"].ToObject<List<string>>();
                    contracts[contractName] = cities;
                }
                return contracts;

            }
            else
            {
                Console.WriteLine("Contracts request failed " + response.StatusCode + " - " + response.ReasonPhrase);
                return null;
            }
        }
        public async Task<string> GetStations(string contractName) // récupère les stations format json
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url + "stations?contract=" + contractName + "&apiKey=" + apiKey);
            Debug.WriteLine("JCDecauxClient.cs");
            Debug.WriteLine(url + "stations?contract=" + contractName + "&apiKey=" + apiKey);
            if (response.IsSuccessStatusCode)
            {
                string responseMessage = await response.Content.ReadAsStringAsync();
                return responseMessage;
            }
            else
            {
                Console.WriteLine("Stations request failed " + response.StatusCode + " - " + response.ReasonPhrase);
                return null;
            }
        }
    }
}