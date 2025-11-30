using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy.valueobjects
{
    public class Contract
    {

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("commercial_name")]
        public string commercial_name { get; set; }

        [JsonProperty("cities")]
        public List<string> cities { get; set; }

        [JsonProperty("country_code")]
        public string country_code { get; set; }

        public Contract() { }

        public void init(Contract other)
        {
            this.name = other.name;
            this.commercial_name = other.commercial_name;
            this.cities = other.cities;
            this.country_code = other.country_code;
        }
    }
}
