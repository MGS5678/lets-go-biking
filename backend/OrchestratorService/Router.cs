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

        public Station GetNearestStation(List<Station> stations, string coordsPosition)
        {
            if (string.IsNullOrEmpty(coordsPosition))
                return null;

            var addressCoords = JsonConvert.DeserializeObject<double[]>(coordsPosition);
            double addressLng = addressCoords[0];
            double addressLat = addressCoords[1];


            if (stations == null || !stations.Any())
                return null;



            Station nearestStation = null;
            double minDistance = double.MaxValue;

            foreach (var station in stations)
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

        public Station GetNearestStationWithContract(string coordsPosition, List<Station> stations, string contractName)
        {
            if (string.IsNullOrEmpty(contractName))
                return null;
            List<Station> stationsInContract = stations.Where(s => s.status == contractName).ToList();
            return GetNearestStation(stationsInContract, coordsPosition);
        }

        public double GetDuration(string route)
        {
            JObject obj = JObject.Parse(route);
            JArray features = (JArray)obj["features"];
            JObject firstFeature = (JObject)features.First;
            JObject segments = (JObject)firstFeature["properties"]["segments"].First;
            double dureeTrajet = (double)segments["duration"];
            return dureeTrajet;
        }

        public List<string> GetShortestRoute(List<List<string>> trajets)
        {
            List<double> durees = new List<double>();
            foreach (List<string> trajet in trajets)
            {
                double dureeTotale = 0;
                foreach (string segment in trajet)
                {
                    dureeTotale += GetDuration(segment);
                }
                durees.Add(dureeTotale);
            }
            int indexMin = durees.IndexOf(durees.Min());
            return trajets[indexMin];

        }

        public async Task<string> GetRoute(string address1, string address2) //changer les paramètres pour pouvoir lutiliser sur plusieurs trajets
        {
            Debug.WriteLine("Router.cs - GetRoute called");

            string coordsDepart = await _proxyClient.GetCoords(address1);
            string coordsArrivee = await _proxyClient.GetCoords(address2);

            List<Station> allStations = JsonConvert.DeserializeObject<List<Station>>(await _proxyClient.GetAllStations());

            Station stationD = GetNearestStation(allStations, coordsDepart);
            Station stationA = GetNearestStation(allStations, coordsArrivee);

            List<List<string>> possibleRoutes = new List<List<string>>();
            List<string> trajetLePlusCourt;
            List<string> trajetOnFoot = new List<string> { await _proxyClient.GetRoute(coordsDepart, coordsArrivee, "foot") };
            possibleRoutes.Add(trajetOnFoot);

            string trajetD_SD = await _proxyClient.GetRoute(coordsDepart, stationD.position.ToString(), "foot");
            string trajetSA_A = await _proxyClient.GetRoute(stationA.position.ToString(), coordsArrivee, "foot");

            // cas same contract
            if (stationD.contract_name == stationA.contract_name)
            {
                string trajetInterStations = await _proxyClient.GetRoute(stationD.position.ToString(), stationA.position.ToString(), "bike");
                List<string> trajetComplet = new List<string> { trajetD_SD, trajetInterStations, trajetSA_A };
                possibleRoutes.Add(trajetComplet);
                trajetLePlusCourt = GetShortestRoute(possibleRoutes);
                return "[" + string.Join(",", trajetLePlusCourt) + "]";
            }
            // cas different contract

            Station stationD2 = GetNearestStationWithContract(coordsArrivee, allStations, stationD.contract_name);
            Station stationA2 = GetNearestStationWithContract(stationD2.position.ToString(), allStations, stationA.contract_name);
            // pourquoi ne pas plutot ou aussi faire stationA2 = GetNearestStationWithContract(coordsDepart, allStations, stationA.contract_name); ?
            // et stationD2 = GetNearestStationWithContract(stationA2.position.ToString(), allStations, stationD.contract_name); ?

            string trajetSD_SD2 = await _proxyClient.GetRoute(stationD.position.ToString(), stationD2.position.ToString(), "bike");
            string trajetSD2_A = await _proxyClient.GetRoute(stationD2.position.ToString(), coordsArrivee, "foot");
            string trajetSD2_SA2 = await _proxyClient.GetRoute(stationD2.position.ToString(), stationA2.position.ToString(), "foot");
            string trajetSA2_SA = await _proxyClient.GetRoute(stationA2.position.ToString(), stationA.position.ToString(), "bike");
            string trajetD_SA2 = await _proxyClient.GetRoute(stationD.position.ToString(), stationA2.position.ToString(), "foot");

            List<string> trajetD_SD_SD2_A = new List<string> { trajetD_SD, trajetSD_SD2, trajetSD2_A };
            List<string> trajetD_SD_SD2_SA2_SA_A = new List<string> { trajetD_SD, trajetSD_SD2, trajetSD2_SA2, trajetSA2_SA, trajetSA_A };
            List<string> trajetD_SA2_SA_A = new List<string> { trajetD_SA2, trajetSA2_SA, trajetSA_A };
            possibleRoutes.Add(trajetD_SD_SD2_A);
            possibleRoutes.Add(trajetD_SD_SD2_SA2_SA_A);
            possibleRoutes.Add(trajetD_SA2_SA_A);

            trajetLePlusCourt = GetShortestRoute(possibleRoutes);

            string trajetFinal = "[" + string.Join(",", trajetLePlusCourt) + "]";

            return trajetFinal;

            //if (stationD.number == stationD2.number) { }

            //if (stationA.number == stationA2.number)
            //{
            //    if (stationD.number == stationD2.number)
            //    {
            //        return trajetOnFoot;
            //    }
            //    string trajetSD_SD2 = await _proxyClient.GetRoute(stationD.position.ToString(), stationD2.position.ToString(), "bike");
            //    string trajetSD2_A = await _proxyClient.GetRoute(stationD2.position.ToString(), coordsArrivee, "foot");
            //    return "[" + trajetD_SD + "," + trajetSD_SD2 + "," + trajetSD2_A + "]";
            //}
            //string trajetSD2_Arrivee = await _proxyClient.GetRoute(stationD2.position.ToString(), coordsArrivee, "foot");

            //string trajetSD2_SA2 = await _proxyClient.GetRoute(stationD2.position.ToString(), stationA2.position.ToString(), "foot");
            //string trajetSA2_SA = await _proxyClient.GetRoute(stationA2.position.ToString(), stationA.position.ToString(), "bike");
            //string trajetSA_Arrivee = await _proxyClient.GetRoute(stationA.position.ToString(), coordsArrivee, "foot");

            //double dureeTrajetSD2_Arrivee = GetDuration(trajetSD2_Arrivee);

            //double dureeTrajetSD2_SA2 = GetDuration(trajetSD2_SA2);
            //double dureeTrajetSA2_SA = GetDuration(trajetSA2_SA);
            //double dureeTrajetSA_Arrivee = GetDuration(trajetSA_Arrivee);

            //double dureeTrajetSD2_SA2_SA_Arrivee = dureeTrajetSD2_SA2 + dureeTrajetSA2_SA + dureeTrajetSA_Arrivee;

            //if (dureeTrajetSD2_Arrivee < dureeTrajetSD2_SA2_SA_Arrivee)
            //{
            //    if (stationD.number == stationD2.number)
            //    {
            //        return GetFinalRoute(trajetOnFoot, new List<string> { trajetD_SD, trajetSD2_Arrivee });
            //    }
            //    string trajetSD_SD2 = await _proxyClient.GetRoute(stationD.position.ToString(), stationD2.position.ToString(), "bike");
            //    return GetFinalRoute(trajetOnFoot, new List<string> { trajetD_SD, trajetSD_SD2, trajetSD2_Arrivee });
            //}
            //else
            //{
            //    if (stationD.number == stationD2.number)
            //    {
            //        return GetFinalRoute(trajetOnFoot, new List<string> { trajetD_SD, trajetSD2_SA2, trajetSA2_SA, trajetSA_Arrivee });
            //    }
            //    string trajetSD_SD2 = await _proxyClient.GetRoute(stationD.position.ToString(), stationD2.position.ToString(), "bike");
            //    return GetFinalRoute(trajetOnFoot, new List<string> { trajetD_SD, trajetSD_SD2, trajetSD2_SA2, trajetSA2_SA, trajetSA_Arrivee });
            //}

        }




    }
    //public async Task<string> GetFullRoute(string address1, string address2)
    //{
    //    Debug.WriteLine("Router.cs - GetFullRoute called");
    //    Debug.WriteLine($"Router.cs - GetFullRoute parameters: address1={address1}, address2={address2}");
    //    // premier trajet : entre la adresse 1 et la station la plus proche de l'adresse 1
    //    string coordsDepart = await _proxyClient.GetCoords(address1);
    //    Station stationDepart = await GetNearestStation(coordsDepart);
    //    Debug.WriteLine("Station de départ choisie : " + stationDepart.name + stationDepart.position.ToString());
    //    string trajetStationDepart = await _proxyClient.GetRoute(coordsDepart, stationDepart.position.ToString(), "foot");

    //    // troisième trajet : entre la station la plus proche de l'adresse 2 et l'adresse 2
    //    string coordsArrivee = await _proxyClient.GetCoords(address2);
    //    Station stationArrivee = await GetNearestStation(coordsArrivee);
    //    string trajetStationArrivee = await _proxyClient.GetRoute(stationArrivee.position.ToString(), coordsArrivee, "foot");

    //    // deuxième trajet : entre la station 1 et la station 2
    //    string trajetInterStations = await _proxyClient.GetRoute(stationDepart.position.ToString(), stationArrivee.position.ToString(), "bike");

    //    string trajetOnFoot = await _proxyClient.GetRoute(coordsDepart, coordsArrivee, "foot");

    //    JObject obj = JObject.Parse(trajetOnFoot);
    //    JArray features = (JArray)obj["features"];
    //    JObject firstFeature = (JObject)features.First;
    //    JObject segments = (JObject)firstFeature["properties"]["segments"].First;
    //    double dureeTrajetOnFoot = (double)segments["duration"];

    //    double dureeTrajetComplet = 0;
    //    foreach (string trajet in new string[] { trajetStationDepart, trajetInterStations, trajetStationArrivee })
    //    {
    //        obj = JObject.Parse(trajet);
    //        features = (JArray)obj["features"];
    //        firstFeature = (JObject)features.First;
    //        segments = (JObject)firstFeature["properties"]["segments"].First;

    //        double dureeTrajet = (double)segments["duration"];
    //        dureeTrajetComplet += dureeTrajet;
    //    }

    //    string routeJson;
    //    string mode;

    //    if (dureeTrajetComplet < dureeTrajetOnFoot)
    //    {
    //        var routeObject = new JObject
    //        {
    //            ["trajet1"] = JObject.Parse(trajetStationDepart),
    //            ["trajet2"] = JObject.Parse(trajetInterStations),
    //            ["trajet3"] = JObject.Parse(trajetStationArrivee)
    //        };
    //        routeJson = routeObject.ToString(Formatting.None);
    //        mode = "multimodal";
    //    }
    //    else
    //    {
    //        routeJson = trajetOnFoot;
    //        mode = "foot";
    //    }

    //    var result = new JObject
    //    {
    //        ["route"] = JToken.Parse(routeJson),
    //        ["mode"] = mode
    //    };

    //    return result.ToString(Formatting.None);
    //}

}

