using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ProxyJCDecaux
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class ProxyService : IProxyService
    {
        private static readonly HttpClient SharedHttpClient = new HttpClient();
        public async Task<string> GetContractNameFromCity(string cityName)
        {
            var proxy = new ProxyServer(SharedHttpClient);
            string contractName = await proxy.GetContractNameFromCity(cityName);
            Debug.WriteLine("ProxyService.cs - GetContractNameFromCity - returned contract: " + contractName + " for city: " + cityName);
            return contractName;
        }
        public async Task<string> GetStationsJson(string contractName)
        {
            var proxy = new ProxyServer(SharedHttpClient);
            Debug.WriteLine("ProxyService.cs");
            string stationsJson = await proxy.GetStationsJson(contractName);
            return stationsJson;
        }
    }
}
