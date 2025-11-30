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


        public async Task<string> GetCoords(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return string.Empty;
            var proxy = new ProxyClient(_httpClient);
            string coordsJson = await proxy.GetCoords(address);
            return coordsJson;
        }


        public async Task<string> GetRouteFromAddresses(string address1, string address2)
        {
            Debug.WriteLine("OrchestratorService.cs - GetRouteFromAddresses called");
            Debug.WriteLine($"OrchestratorService.cs - GetRouteFromAddresses parameters: address1={address1}, address2={address2}");
            if (string.IsNullOrWhiteSpace(address1) || string.IsNullOrWhiteSpace(address2))
                return string.Empty;
            string routeJson = await router.GetRoute(address1, address2);
            return routeJson;
        }
    }
}