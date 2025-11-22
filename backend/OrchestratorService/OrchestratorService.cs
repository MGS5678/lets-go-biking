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

        public OrchestratorService()
        {
            proxy = new ProxyClient(_httpClient);
        }


        public async Task<string> GetContractNameFromCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return string.Empty;
            var proxy = new ProxyClient(_httpClient);
            string contractName = await proxy.GetContractNameFromCity(city);
            Debug.WriteLine("OrchestratorService.cs - GetContractNameFromCity - returned contract: " + contractName + " for city: " + city);
            return contractName;
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
    }
}