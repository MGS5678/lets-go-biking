using Newtonsoft.Json;
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

        public OrchestratorService()
        {
            proxy = new ProxyClient(_httpClient);
            router = new Router();
        }


        public async Task<string> GetContractNameFromCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return string.Empty;
            var proxy = new ProxyClient(_httpClient);
            string contractName = await proxy.GetContractNameFromCity(city);
            Debug.WriteLine("OrchestratorService.cs - GetContractNameFromCity - returned contract: " + contractName + " for city: " + city);
            return contractName.Trim('"');
        }

        public async Task<string> GetStations(string contract)
        {
            if (string.IsNullOrWhiteSpace(contract))
                return string.Empty;

            var proxy = new ProxyClient(_httpClient);
            Debug.WriteLine("OrchestratorService.cs");
            List<Station> stations = await proxy.GetStations(contract);

            if (stations == null)
                return "[]";
            return JsonConvert.SerializeObject(stations);

        }

        public async Task<string> GetCoords(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return string.Empty;
            var proxy = new ProxyClient(_httpClient);
            string coordsJson = await proxy.GetCoords(address);
            return coordsJson;
        }

        public async Task<string> GetRoute(string coords1, string coords2, string meansTransport)
        {
            Debug.WriteLine("OrchestratorService.cs - GetRoute called");
            Debug.WriteLine($"OrchestratorService.cs - GetRoute parameters: coords1={coords1}, coords2={coords2}, meansTransport={meansTransport}");
            if (string.IsNullOrWhiteSpace(coords1) || string.IsNullOrWhiteSpace(coords2) || string.IsNullOrWhiteSpace(meansTransport))
                return string.Empty;
            var proxy = new ProxyClient(_httpClient);
            string routeJson = await proxy.GetRoute(coords1, coords2, meansTransport);
            return routeJson;
        }

        public async Task<string> GetRouteFromAddresses(string address1, string address2)
        {
            Debug.WriteLine("OrchestratorService.cs - GetRouteFromAddresses called");
            Debug.WriteLine($"OrchestratorService.cs - GetRouteFromAddresses parameters: address1={address1}, address2={address2}");
            if (string.IsNullOrWhiteSpace(address1) || string.IsNullOrWhiteSpace(address2))
                return string.Empty;
            string routeJson = await router.GetFullRoute(address1, address2);
            return routeJson;
        }
    }
}