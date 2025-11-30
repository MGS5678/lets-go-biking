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

    public class JCDecauxClient
    {
        HttpClient _httpClient;
        string apiKey = "056bfe4ddaac72f8acb8292991c535938fc511d7";// "668bf3fd91bde76a99077bb91ec0c543339a25b2";
        string url = "https://api.jcdecaux.com/vls/v1/";
        public JCDecauxClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<Station>> GetAllStations() // recup les stations au format json 
        {
            //string requestUrl = "https://api.jcdecaux.com/vls/v3/" + "stations?apiKey=" + apiKey;
            string requestUrl = url + "stations?apiKey=" + apiKey;
            Debug.WriteLine("JCDecauxClient.cs");
            Debug.WriteLine(requestUrl);
            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseMessage = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("JCDecauxClient.cs - GetAllStations - returned all stations");
                return JsonConvert.DeserializeObject<List<Station>>(responseMessage);
            }
            else
            {
                Console.WriteLine("Stations request failed " + response.StatusCode + " - " + response.ReasonPhrase);
                return null;
            }
        }
    }
}