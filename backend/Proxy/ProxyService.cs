using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class ProxyService : IProxyService
    {
        private static readonly HttpClient SharedHttpClient = new HttpClient();
        private static ProxyServer proxy = new ProxyServer(SharedHttpClient);
        public async Task<string> GetContractNameFromCity(string cityName)
        {
            string contractName = await proxy.GetContractNameFromCity(cityName);
            Debug.WriteLine("ProxyService.cs - GetContractNameFromCity - returned contract: " + contractName + " for city: " + cityName);
            return contractName;
        }
        public async Task<string> GetStationsJson(string contractName)
        {
            Debug.WriteLine("ProxyService.cs");
            string stationsJson = await proxy.GetStationsJson(contractName);
            return stationsJson;
        }

        public async Task<string> GetCoordsJson(string address)
        {
            string coordsJson = await proxy.GetCoordsJson(address);
            return coordsJson;
        }

        public async Task<string> GetRoute(string coords1, string coords2, string meansTransport)
        {
            Debug.WriteLine("ProxyService.cs - GetRoute called");
            string routeJson = await proxy.GetRoute(coords1, coords2, meansTransport);
            Debug.WriteLine("ProxyService.cs - GetRoute - returned routeJson");
            return routeJson;
        }

        public async Task<string> GetAllStations()
        {
            string allStationsJson = await proxy.GetAllStations();
            Debug.WriteLine("ProxyService.cs - GetAllStations - returned allStationsJson");
            return allStationsJson;
        }
    }
}
