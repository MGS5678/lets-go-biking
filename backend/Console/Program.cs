using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        static async Task MainAsync(string[] args)
        {
            HttpClient client = new HttpClient();

            var defaultBase = "http://localhost:8733/Design_Time_Addresses/OrchestratorService/OrchestratorService";

            //Console.WriteLine("Enter the city's name : ");
            //var cityName = Console.ReadLine();

            //var requestUrl = $"{defaultBase}/contracts?cityName={cityName}";
            //Console.WriteLine($"\n\nGetting contract name for city : {cityName}\n");
            //Console.WriteLine($"Contract Name Request URL : {requestUrl}\n");

            //try
            //{
            //    var respons = await client.GetAsync(requestUrl);

            //    // AJOUTEZ CECI POUR VOIR EXACTEMENT CE QUI SE PASSE
            //    Console.WriteLine($"Status Code: {respons.StatusCode}");
            //    Console.WriteLine($"Reason Phrase: {respons.ReasonPhrase}");

            //    if (!respons.IsSuccessStatusCode)
            //    {
            //        var errorContent = await respons.Content.ReadAsStringAsync();
            //        Console.WriteLine($"Error Content: {errorContent}");
            //    }

            //    respons.EnsureSuccessStatusCode();
            //    var contractName = await respons.Content.ReadAsStringAsync();

            //    // AJOUTEZ CES LOGS POUR VOIR EXACTEMENT CE QUI ARRIVE
            //    Console.WriteLine($"Raw response: [{contractName}]");
            //    Console.WriteLine($"Response length: {contractName.Length}");
            //    Console.WriteLine($"First char: [{contractName[0]}]");
            //    Console.WriteLine($"Last char: [{contractName[contractName.Length - 1]}]");

            //    Debug.WriteLine("ConsoleClient.cs - MainAsync - returned contract: " + contractName + " for city: " + cityName);
            //    Console.WriteLine($"Response from contractsservice : {contractName}");



            //    requestUrl = $"{defaultBase}/stations?contract={contractName}";
            //    Console.WriteLine($"\n\nGetting stations for contract : {contractName}\n");
            //    Console.WriteLine($"Stations Request URL : {requestUrl}\n");

            //    respons = await client.GetAsync(requestUrl);
            //    Debug.WriteLine("A");
            //    var stationsJson = await respons.Content.ReadAsStringAsync();
            //    Debug.WriteLine("B");
            //    Console.WriteLine(stationsJson);
            //    respons.EnsureSuccessStatusCode();

            //    Console.WriteLine($"Response from stationsservice : {stationsJson.Substring(0, 150)}");
            //}
            //catch (HttpRequestException ex)
            //{
            //    Console.WriteLine($"HTTP Error: {ex.Message}");
            //}


            Console.WriteLine("next");
            Console.ReadKey();
            //Console.WriteLine("entrez une adresse : ");
            //var address = Console.ReadLine();
            //requestUrl = $"{defaultBase}/coords?address={address}";
            //Console.WriteLine($"\n\nGetting coordinates for address : {address}\n");
            //Console.WriteLine($"Coordinates Request URL : {requestUrl}\n");
            //string coordsJson1;

            //var response = await client.GetAsync(requestUrl);
            //response.EnsureSuccessStatusCode();
            //coordsJson1 = await response.Content.ReadAsStringAsync();
            //Debug.WriteLine("ConsoleClient.cs - MainAsync - returned coords for address: " + address);
            //Console.WriteLine($"Response from coordservice : {coordsJson1}");



            //Console.WriteLine("entrez une adresse : ");
            //address = Console.ReadLine();
            //requestUrl = $"{defaultBase}/coords?address={address}";
            //Console.WriteLine($"\n\nGetting coordinates for address : {address}\n");
            //Console.WriteLine($"Coordinates Request URL : {requestUrl}\n");
            //string coordsJson2;

            //response = await client.GetAsync(requestUrl);
            //response.EnsureSuccessStatusCode();
            //coordsJson2 = await response.Content.ReadAsStringAsync();
            //Debug.WriteLine("ConsoleClient.cs - MainAsync - returned coords for address: " + address);
            //Console.WriteLine($"Response from coordservice : {coordsJson2}");


            //Console.WriteLine("voici le trajet en vélo entre les deux adresses : ");
            //var routeRequestUrl = $"{defaultBase}/route?coords1={Uri.EscapeDataString(coordsJson1)}&coords2={Uri.EscapeDataString(coordsJson2)}&meansTransport=cycling-regular";
            //Console.WriteLine($"\n\nGetting route between the two coordinates\n");
            //Console.WriteLine($"Route Request URL : {routeRequestUrl}\n");
            //response = await client.GetAsync(routeRequestUrl);
            //response.EnsureSuccessStatusCode();
            //var routeJson = await response.Content.ReadAsStringAsync();
            //Debug.WriteLine("ConsoleClient.cs - MainAsync - returned route between coords");
            //Console.WriteLine($"Response from routeservice : {routeJson}");

            //while (true)
            //{
            //    Console.WriteLine("route");
            //    Console.WriteLine("entrez une adresse de départ : ");
            //    var address1 = Console.ReadLine();
            //    Console.WriteLine("entrez une adresse d'arrivée : ");
            //    var address2 = Console.ReadLine();

            //    Console.WriteLine($"\n\nGetting route between addresses : {address1} and {address2}\n");
            //    var routeUrl = $"{defaultBase}/route?address1={Uri.EscapeDataString(address1)}&address2={Uri.EscapeDataString(address2)}";
            //    Console.WriteLine($"Route from Addresses Request URL : {routeUrl}\n");
            //    var response = await client.GetAsync(routeUrl);
            //    response.EnsureSuccessStatusCode();
            //    var routeJson = await response.Content.ReadAsStringAsync();
            //    Debug.WriteLine("ConsoleClient.cs - MainAsync - returned route between addresses");
            //    Console.WriteLine($"Response from routeservice : {routeJson}");

            //}
            Console.WriteLine("lat:");
            string lat = Console.ReadLine();
            Console.WriteLine("long:");
            string lon = Console.ReadLine();
            string coords = $"[{lat},{lon}]";
            var requestUrl = $"{defaultBase}/meteo?coords={coords}";
            Console.WriteLine($"\n\nGetting meteo for : {coords}\n");
            Console.WriteLine($"meteo requete url : {requestUrl}\n");
            var response = await client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();
            var meteoJson = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("ConsoleClient.cs - MainAsync - returned meteo for coords");
            Console.WriteLine($"Response from meteoservice : {meteoJson}");

        }
    }
}
