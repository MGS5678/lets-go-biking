using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy.valueobjects
{
    public class Meteo
    {
        public Meteo() { }

        [JsonProperty("latitude")]
        public double latitude
        {
            set; get;
        }

        [JsonProperty("longitude")]
        public double longitude
        {
            set; get;
        }



        [JsonProperty("current_weather")]
        public CurrentWeatherData CurrentWeather { get; set; }
        public class CurrentWeatherData
        {
            //[JsonProperty("time")] pas utilisé pour l'instant
            //public string Time { get; set; }
            [JsonProperty("temperature")]
            public double Temperature { get; set; }
            [JsonProperty("windspeed")]
            public double Windspeed { get; set; }

        }



        [JsonProperty("current_weather_units")]
        public CurrentWeatherUnitsData CurrentWeatherUnits { get; set; }
        public class CurrentWeatherUnitsData
        {
            [JsonProperty("temperature")]
            public string Temperature { get; set; }
            [JsonProperty("windspeed")]
            public string Windspeed { get; set; }
        }

        public bool IsEmpty()
        {
            return CurrentWeather == null &&
                   CurrentWeatherUnits == null &&
                   latitude == 0.0 &&
                   longitude == 0.0;
        }
    }
}
