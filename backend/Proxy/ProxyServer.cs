using Newtonsoft.Json;
using Proxy.valueobjects;
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
        private readonly JCDecauxClient _jcdecauxClient;
        private readonly OpenRouteClient _openRouteClient;

        private readonly GenericCache<ContractName> _citiesContractCache;
        private readonly GenericCache<List<Station>> _stationsCache;
        private readonly GenericCache<Coordinate> _coordinatesCache;
        private readonly GenericCache<Route> _routesCache;
        private readonly GenericCache<List<Station>> _allStationsCache;
        //private static readonly HttpClient SharedHttpClient = new HttpClient();

        public ProxyServer(HttpClient httpClient)
        {
            _httpClient = httpClient; //?? SharedHttpClient;
            _jcdecauxClient = new JCDecauxClient(httpClient);
            _openRouteClient = new OpenRouteClient(httpClient);

            _citiesContractCache = new GenericCache<ContractName>();
            _stationsCache = new GenericCache<List<Station>>();
            _coordinatesCache = new GenericCache<Coordinate>();
            _routesCache = new GenericCache<Route>();
            _allStationsCache = new GenericCache<List<Station>>();
        }

        // cache doesn't work bc of the redef of the reference instead of changing it inside
        public async Task<ContractName> GetContractNameFromCity(string cityName) //ici implémenter le cache
        {
            ContractName contractName = _citiesContractCache.Get(cityName);
            if (contractName.Name != null)  // if city's contract cached
            {
                Debug.WriteLine("############# CACHE UTILISE #############");
                Debug.WriteLine(cityName, contractName.Name);
                return contractName;
            }

            // if city's contract not cached, we update all the cities contract
            List<Contract> contracts = await _jcdecauxClient.GetContracts();
            foreach (Contract contract in contracts)
            {
                if (contract.name != null)  // faut que l'obj soit bien instancié
                {
                    foreach (string city in contract.cities)
                    {
                        ContractName newContractName = _citiesContractCache.Get(city);  // new ContractName object, bc of the first method's if
                        newContractName.init(contract.name);
                    }
                }
            }

            // new check to see if the name is now available
            contractName = _citiesContractCache.Get(cityName);
            if (contractName.Name != null)  // if city's contract cached
            {
                Debug.WriteLine("############# REQUETE JCDECAUX #############");
                Debug.WriteLine(cityName, contractName.Name);
                return contractName;
            }

            return null; // ptet mettre autre chose comme (false, null)
        }

        // cache doesn't work bc of the redef of the reference instead of changing it inside
        public async Task<List<Station>> GetStationsJson(string contractName) //ici implémenter le cache
        {
            List<Station> stations = _stationsCache.Get(contractName);
            if (stations.Count != 0)  // not initialized
            {
                Debug.WriteLine("############# CACHE UTILISE #############");
                return stations;
            }

            Debug.WriteLine("############# REQUETE JCDECAUX #############");
            stations = await _jcdecauxClient.GetStations(contractName);  // ####### HERE #######
            return stations;
        }

        public async Task<Coordinate> GetCoordsJson(string address)
        {
            Coordinate coords = _coordinatesCache.Get(address);
            if (coords.lat != 0.0 && coords.lng != 0.0)
            {
                Debug.WriteLine("############# CACHE UTILISE #############");
                return coords;
            }

            Debug.WriteLine("############# REQUETE OPENROUTE #############");
            Coordinate newCoords = await _openRouteClient.GetCoordinates(address);
            coords.lat = newCoords.lat;
            coords.lng = newCoords.lng;
            return coords;
        }

        public async Task<string> GetRoute(string coords1, string coords2, string meansTransport)
        {
            Debug.WriteLine("ProxyServer.cs - GetRoute called");
            string cacheKey = $"{coords1}-{coords2}-{meansTransport}";
            Route route = _routesCache.Get(cacheKey);
            Debug.WriteLine("ROUUUUUUUUTE \"" + route.data + "\"");
            Debug.WriteLine("ROUUUUUUUTE L " + _routesCache.ToString());

            if (route.data != null)
            {
                Debug.WriteLine("############# CACHE UTILISE #############");
                return route.data;
            }

            Debug.WriteLine("############# REQUETE OPENROUTE #############");
            route.data = await _openRouteClient.GetRoute(coords1, coords2, meansTransport);
            return route.data;
        }

        public async Task<List<Station>> GetAllStations()
        {
            List<Station> stations = _allStationsCache.Get("all_stations");
            if (stations.Count != 0)
            {
                Debug.WriteLine("############# CACHE UTILISE #############");
                return stations;
            }

            Debug.WriteLine("############# REQUETE JCDECAUX #############");
            List<Station> newStations = await _jcdecauxClient.GetAllStations();
            foreach (Station s in newStations) { stations.Add(s); }
            Debug.WriteLine("ProxyServer.cs - GetAllStations - returned all stations");
            return stations;
        }
    }
}