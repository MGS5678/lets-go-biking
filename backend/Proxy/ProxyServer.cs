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
        private readonly OpenMeteoClient _openMeteoClient;

        private readonly GenericCache<Coordinate> _coordinatesCache;
        private readonly GenericCache<Route> _routesCache;
        private readonly GenericCache<List<Station>> _allStationsCache;
        private readonly GenericCache<Meteo> _meteoCache;
        //private static readonly HttpClient SharedHttpClient = new HttpClient();

        public ProxyServer(HttpClient httpClient)
        {
            _httpClient = httpClient; //?? SharedHttpClient;
            _jcdecauxClient = new JCDecauxClient(_httpClient);
            _openRouteClient = new OpenRouteClient(_httpClient);
            _openMeteoClient = new OpenMeteoClient(_httpClient);

            _coordinatesCache = new GenericCache<Coordinate>();
            _routesCache = new GenericCache<Route>();
            _allStationsCache = new GenericCache<List<Station>>();
            _meteoCache = new GenericCache<Meteo>();
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
            if (newCoords == null) return null;
            coords.lat = newCoords.lat;
            coords.lng = newCoords.lng;
            return coords;
        }

        public async Task<string> GetRoute(string coords1, string coords2, string meansTransport)
        {
            Debug.WriteLine("ProxyServer.cs - GetRoute called");
            string cacheKey = $"{coords1}-{coords2}-{meansTransport}";
            Route route = _routesCache.Get(cacheKey);

            if (route.data != null)
            {
                Debug.WriteLine("############# CACHE UTILISE #############");
                return route.data;
            }

            Debug.WriteLine("############# REQUETE OPENROUTE #############");
            string temp = await _openRouteClient.GetRoute(coords1, coords2, meansTransport);
            if (temp == null) return "";
            route.data = temp;
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
            Debug.WriteLine($"first station : " + JsonConvert.SerializeObject(newStations[0]));
            foreach (Station s in newStations)
            {
                stations.Add(s);
            }
            Debug.WriteLine("ProxyServer.cs - GetAllStations - returned all stations");
            return stations;
        }

        public async Task<Meteo> GetMeteoFromCoords(string coords)
        {
            string idforcache = $"{coords}";
            Meteo meteo = _meteoCache.Get(idforcache, 15);
            if (!meteo.IsEmpty())
            {
                Debug.WriteLine("############# CACHE UTILISE #############");
                return meteo;
            }

            Debug.WriteLine("############# REQUETE JCDECAUX #############");
            Meteo newMeteo = await _openMeteoClient.GetMeteoFromCoords(coords);

            meteo.latitude = newMeteo.latitude;
            meteo.longitude = newMeteo.longitude;
            meteo.CurrentWeather = newMeteo.CurrentWeather;
            meteo.CurrentWeatherUnits = newMeteo.CurrentWeatherUnits;

            return meteo;
        }
    }
}