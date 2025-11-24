using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace OrchestratorService
{
    public class Router
    {
        ProxyClient _proxyClient;
        HttpClient _httpClient;
        public Router()
        {
            _httpClient = new HttpClient();
            _proxyClient = new ProxyClient(_httpClient);
        }

        public async Task<Station> GetNearestStation(string coordsPosition)
        {
            if (string.IsNullOrEmpty(coordsPosition))
                return null;

            var addressCoords = JsonConvert.DeserializeObject<double[]>(coordsPosition);
            double addressLng = addressCoords[0];
            double addressLat = addressCoords[1];

            string allStationsJson = await _proxyClient.GetAllStations();

            if (string.IsNullOrEmpty(allStationsJson))
                return null;

            var allStations = JsonConvert.DeserializeObject<List<Station>>(allStationsJson);

            if (allStations == null || !allStations.Any())
                return null;

            Station nearestStation = null;
            double minDistance = double.MaxValue;

            foreach (var station in allStations)
            {
                if (station.position != null) // amélioration possible : faire les requetes de trajet à pied par requete Direction de openroute à la place de la distance à vol d'oiseau
                {
                    double distance = station.position.DistanceTo(addressLat, addressLng);
                    ;
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestStation = station;
                    }
                }
            }
            Debug.WriteLine("Distance à la station la plus proche : " + minDistance + " km");
            Debug.WriteLine("Coordonnées de la station la plus proche : " + nearestStation.position.ToString());
            return nearestStation != null ? nearestStation : null;
        }
        public async Task<string> GetFullRoute(string address1, string address2)
        {
            Debug.WriteLine("Router.cs - GetFullRoute called");
            Debug.WriteLine($"Router.cs - GetFullRoute parameters: address1={address1}, address2={address2}");
            // premier trajet : entre la adresse 1 et la station la plus proche de l'adresse 1
            string coordsDepart = await _proxyClient.GetCoords(address1);
            Station stationDepart = await GetNearestStation(coordsDepart);
            Debug.WriteLine("Station de départ choisie : " + stationDepart.name + stationDepart.position.ToString());
            string trajetStationDepart = await _proxyClient.GetRoute(coordsDepart, stationDepart.position.ToString(), "foot");

            // troisième trajet : entre la station la plus proche de l'adresse 2 et l'adresse 2
            string coordsArrivee = await _proxyClient.GetCoords(address2);
            Station stationArrivee = await GetNearestStation(coordsArrivee);
            string trajetStationArrivee = await _proxyClient.GetRoute(stationArrivee.position.ToString(), coordsArrivee, "foot");

            // deuxième trajet : entre la station 1 et la station 2
            string trajetInterStations = await _proxyClient.GetRoute(stationDepart.position.ToString(), stationArrivee.position.ToString(), "bike");

            //string trajetOnFoot = await _proxyClient.GetRoute(stationDepart.position.ToString(), stationArrivee.position.ToString(), "foot");
            string trajetOnFoot = await _proxyClient.GetRoute(coordsDepart, coordsArrivee, "foot");
            
            JObject obj = JObject.Parse(trajetOnFoot);
            JArray features = (JArray)obj["features"];
            JObject firstFeature = (JObject)features.First;
            JObject segments = (JObject)firstFeature["properties"]["segments"].First;
            double dureeTrajetOnFoot = (double)segments["duration"];

            double dureeTrajetComplet = 0;
            foreach (string trajet in new string[] { trajetStationDepart, trajetInterStations, trajetStationArrivee })
            {
                obj = JObject.Parse(trajet);
                features = (JArray)obj["features"];
                firstFeature = (JObject)features.First;
                segments = (JObject)firstFeature["properties"]["segments"].First;

                double dureeTrajet = (double)segments["duration"];
                dureeTrajetComplet += dureeTrajet;
            }

            string routeJson;
            string mode;

            if (dureeTrajetComplet < dureeTrajetOnFoot)
            {
                var routeObject = new JObject
                {
                    ["trajet1"] = JObject.Parse(trajetStationDepart),
                    ["trajet2"] = JObject.Parse(trajetInterStations),
                    ["trajet3"] = JObject.Parse(trajetStationArrivee)
                };
                routeJson = routeObject.ToString(Formatting.None);
                mode = "multimodal";
            }
            else
            {
                routeJson = trajetOnFoot;
                mode = "foot";
            }

            var result = new JObject
            {
                ["route"] = JToken.Parse(routeJson),
                ["mode"] = mode
            };

            return result.ToString(Formatting.None);
        }

    }
}
