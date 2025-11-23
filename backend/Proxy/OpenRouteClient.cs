using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class OpenRouteClient
    {
        HttpClient _httpClient;
        string apiKey = "eyJvcmciOiI1YjNjZTM1OTc4NTExMTAwMDFjZjYyNDgiLCJpZCI6ImQ0MWYyZTg4N2Q4MDQwNWU5ZWQxY2U2MmRhOTNkNGNkIiwiaCI6Im11cm11cjY0In0 =";
        string url = "https://api.openrouteservice.org/geocode/";
        public OpenRouteClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
