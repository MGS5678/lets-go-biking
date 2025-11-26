using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Proxy
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
                Debug.WriteLine(cityName, _proxyCache.Get(cityName) as string);
                return _proxyCache.Get(cityName) as string;
            }
            var server = new JCDecauxClient(_httpClient);
            Dictionary<string, List<string>> contracts = await server.GetContracts();
            foreach (KeyValuePair<string, List<string>> kv in contracts)
            {
                if (kv.Value != null)
                {
                    foreach (string city in kv.Value)
                    {
                        _proxyCache.Set(city, kv.Key);
                    }

                }
                else
                {
                    Debug.WriteLine(char.ToUpper(kv.Key[0]) + kv.Key.Substring(1));
                    _proxyCache.Set(char.ToUpper(kv.Key[0]) + kv.Key.Substring(1), kv.Key);
                }
            }
            if (_proxyCache.Get(cityName) != default)
            {
                Debug.WriteLine("############# REQUETE JCDECAUX #############");
                Debug.WriteLine(cityName, _proxyCache.Get(cityName) as string);
                return _proxyCache.Get(cityName) as string;
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
            return _proxyCache.Get(contractName) as string;
        }

        public async Task<string> GetCoordsJson(string address)
        {
            if (_proxyCache.Get(address) != default)
            {
                Debug.WriteLine("############# CACHE UTILISE #############");
                return _proxyCache.Get(address) as string;
            }
            Debug.WriteLine("############# REQUETE OPENROUTE #############");
            var server = new OpenRouteClient(_httpClient);
            string coordsJson = await server.GetCoordinates(address);
            _proxyCache.Set(address, coordsJson);
            return _proxyCache.Get(address) as string;
        }

        public async Task<string> GetRoute(string coords1, string coords2, string meansTransport)
        {
            Debug.WriteLine("ProxyServer.cs - GetRoute called");
            string cacheKey = $"{coords1}-{coords2}-{meansTransport}";
            if (_proxyCache.Get(cacheKey) != default)
            {
                //Debug.WriteLine("############# CACHE UTILISE #############");
                return _proxyCache.Get(cacheKey) as string;
            }
            //Debug.WriteLine("############# REQUETE OPENROUTE #############");
            var server = new OpenRouteClient(_httpClient);
            string routeJson = await server.GetRoute(coords1, coords2, meansTransport);
            _proxyCache.Set(cacheKey, routeJson);
            return _proxyCache.Get(cacheKey) as string;
        }

        public async Task<string> GetAllStations()
        {
            if (_proxyCache.Get("all_stations") != default)
            {
                Debug.WriteLine("############# CACHE UTILISE #############");
                return _proxyCache.Get("all_stations") as string;
            }
            Debug.WriteLine("############# REQUETE JCDECAUX #############");
            var server = new JCDecauxClient(_httpClient);
            string stationsJson = await server.GetAllStations();
            _proxyCache.Set("all_stations", stationsJson);
            Debug.WriteLine("ProxyServer.cs - GetAllStations - returned all stations");
            return _proxyCache.Get("all_stations") as string;
        }
    }
}