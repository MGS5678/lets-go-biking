using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace OrchestratorService
{
    public class OrchestratorService : IOrchestratorService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ProxyClient proxy;
        private readonly Router router;
        private static bool _activeMqStarted = false;
        private static readonly object _startLock = new object();

        public OrchestratorService()
        {
            proxy = new ProxyClient(_httpClient);
            router = new Router();

            // Démarrer le producteur ActiveMQ une seule fois
            StartActiveMQProducer();
        }

        private void StartActiveMQProducer()
        {
            if (!_activeMqStarted)
            {
                lock (_startLock)
                {
                    if (!_activeMqStarted)
                    {
                        try
                        {
                            ActiveMQProducer.Instance.Start();
                            _activeMqStarted = true;
                            Debug.WriteLine("[OrchestratorService] ActiveMQ Producer démarré");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[OrchestratorService] Erreur démarrage ActiveMQ: {ex.Message}");
                        }
                    }
                }
            }
        }

        public async Task<string> GetRouteFromAddresses(string address1, string address2)
        {
            Debug.WriteLine("OrchestratorService.cs - GetRouteFromAddresses called");
            Debug.WriteLine($"OrchestratorService.cs - GetRouteFromAddresses parameters: address1={address1}, address2={address2}");
            if (string.IsNullOrWhiteSpace(address1) || string.IsNullOrWhiteSpace(address2))
                return string.Empty;
            string routeJson = await router.GetRoute(address1, address2);

            UpdateWaypointsFromRoute(routeJson);

            return routeJson;
        }

        private void UpdateWaypointsFromRoute(string routeJson)
        {
            try
            {
                JArray array = JArray.Parse(routeJson);
                List<Coordinate> allWaypoints = new List<Coordinate>();

                foreach (var item in array)
                {
                    var coords = item["metadata"]?["query"]?["coordinates"] as JArray;
                    if (coords != null)
                    {
                        foreach (var coord in coords)
                        {
                            var coordArray = coord as JArray;
                            if (coordArray != null && coordArray.Count >= 2)
                            {
                                var waypoint = new Coordinate
                                {
                                    lat = (double)coordArray[1],
                                    lng = (double)coordArray[0]
                                };
                                allWaypoints.Add(waypoint);
                                Debug.WriteLine($"[OrchestratorService] Waypoint ajouté: ({waypoint.lat},{waypoint.lng})");
                            }
                        }
                    }
                }

                RouteWaypoints.Instance.SetWaypoints(allWaypoints);
                Debug.WriteLine($"[OrchestratorService] {allWaypoints.Count} waypoints mis à jour au total");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OrchestratorService] Erreur UpdateWaypoints: {ex.Message}");
            }
        }

        public async Task<string> GetMeteoFromCoords(string coords)
        {
            Debug.WriteLine("OrchestratorService.cs - GetMeteoFromCoords called");
            if (string.IsNullOrWhiteSpace(coords))
            {
                Debug.WriteLine("OrchestratorService.cs - GetMeteoFromCoords - coords parameter is null or empty");
                return string.Empty;
            }
            string meteoJson = await proxy.GetMeteo(coords);
            return meteoJson;
        }

        public string GetRouteWayPoints()
        {
            var waypoints = RouteWaypoints.Instance.GetWaypoints();
            return JsonConvert.SerializeObject(waypoints);
        }

        /// <summary>
        /// Efface les waypoints (à appeler quand l'utilisateur a terminé son trajet)
        /// </summary>
        public void ClearWaypoints()
        {
            RouteWaypoints.Instance.Clear();
        }
    }
}