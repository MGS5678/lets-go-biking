using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProxyJCDecaux
{
    public class ProxyServer
    {
        private readonly HttpClient _httpClient;
        //private static readonly HttpClient SharedHttpClient = new HttpClient();

        public ProxyServer(HttpClient httpClient)
        {
            _httpClient = httpClient; //?? SharedHttpClient;
        }
        public async Task<string> GetContractNameFromCity(string cityName)
        {
            var server = new JCDecauxClient(_httpClient);
            Dictionary<string, List<string>> contracts = await server.getContracts();
            Debug.WriteLine(contracts);
            foreach (KeyValuePair<string, List<string>> kv in contracts)
            {
                if (kv.Value != null && kv.Value.Contains(cityName))
                {
                    return kv.Key;
                }
            }
            return null; // ptet mettre autre chose comme (false, null)

        }

        public async Task<string> GetStationsJson(string contractName)
        {
            var server = new JCDecauxClient(_httpClient);
            string stationsJson = await server.getStations(contractName);
            return stationsJson;
        }

    }
}