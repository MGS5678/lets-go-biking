using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Proxy
{

    public class JCDecauxClient
    {
        HttpClient _httpClient;
        string apiKey = "056bfe4ddaac72f8acb8292991c535938fc511d7";// "668bf3fd91bde76a99077bb91ec0c543339a25b2";
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

        public async Task<string> GetStations(string contractName) // recup les stations au format json 
        {
            string cleanContractName = contractName?.Trim('"') ?? contractName;

            string requestUrl = $"{url}stations?contract={cleanContractName}&apiKey={apiKey}";
            Debug.WriteLine("JCDecauxClient.cs");
            Debug.WriteLine(requestUrl);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseMessage = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("JCDecauxClient.cs - GetStations - returned stations for contract: " + cleanContractName);
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