using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProxyJCDecaux
{
    public class ProxyServer
    {
        private readonly HttpClient _httpClient;
        private readonly GenericProxyCache _proxyCache;
        //private static readonly HttpClient SharedHttpClient = new HttpClient();

        public ProxyServer(HttpClient httpClient)
        {
            _httpClient = httpClient; //?? SharedHttpClient;
            _proxyCache = new GenericProxyCache();
        }
        public async Task<string> GetContractNameFromCity(string cityName) //ici implémenter le cache
        {
            if (_proxyCache.Get(cityName) != default)
            {
                Debug.WriteLine("############# CACHE UTILISE #############");
                return _proxyCache.Get(cityName) as string;
            }
            var server = new JCDecauxClient(_httpClient);
            Dictionary<string, List<string>> contracts = await server.GetContracts();
            foreach (KeyValuePair<string, List<string>> kv in contracts)
            {
                if (kv.Value != null && kv.Value.Contains(cityName))
                {
                    Debug.WriteLine("############# REQUETE JCDECAUX #############");
                    _proxyCache.Set(cityName, kv.Key);
                    return kv.Key;
                }
            }
            return null; // ptet mettre autre chose comme (false, null)

        }

        public async Task<string> GetStationsJson(string contractName) //ici implémenter le cache
        {
            if (_proxyCache.Get(contractName) != default)
            {
                Debug.WriteLine("############# CACHE UTILISE #############");
                return _proxyCache.Get(contractName) as string;
            }
            var server = new JCDecauxClient(_httpClient);
            Debug.WriteLine("############# REQUETE JCDECAUX #############");
            string stationsJson = await server.GetStations(contractName);
            _proxyCache.Set(contractName, stationsJson);
            return stationsJson;
        }

    }
}