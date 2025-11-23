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
            while (true)
            {
                Console.WriteLine("Enter the city's name : ");
                var cityName = Console.ReadLine();

                var requestUrl = $"{defaultBase}/contracts?cityName={cityName}";
                Console.WriteLine($"\n\nGetting contract name for city : {cityName}\n");
                Console.WriteLine($"Contract Name Request URL : {requestUrl}\n");

                try
                {
                    var response = await client.GetAsync(requestUrl);

                    // AJOUTEZ CECI POUR VOIR EXACTEMENT CE QUI SE PASSE
                    Console.WriteLine($"Status Code: {response.StatusCode}");
                    Console.WriteLine($"Reason Phrase: {response.ReasonPhrase}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error Content: {errorContent}");
                    }

                    response.EnsureSuccessStatusCode();
                    var contractName = await response.Content.ReadAsStringAsync();

                    // AJOUTEZ CES LOGS POUR VOIR EXACTEMENT CE QUI ARRIVE
                    Console.WriteLine($"Raw response: [{contractName}]");
                    Console.WriteLine($"Response length: {contractName.Length}");
                    Console.WriteLine($"First char: [{contractName[0]}]");
                    Console.WriteLine($"Last char: [{contractName[contractName.Length - 1]}]");

                    Debug.WriteLine("ConsoleClient.cs - MainAsync - returned contract: " + contractName + " for city: " + cityName);
                    Console.WriteLine($"Response from contractsservice : {contractName}");



                    requestUrl = $"{defaultBase}/stations?contract={contractName}";
                    Console.WriteLine($"\n\nGetting stations for contract : {contractName}\n");
                    Console.WriteLine($"Stations Request URL : {requestUrl}\n");

                    response = await client.GetAsync(requestUrl);
                    Debug.WriteLine("A");
                    var stationsJson = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("B");
                    Console.WriteLine(stationsJson);
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine($"Response from stationsservice : {stationsJson.Substring(0, 150)}");
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP Error: {ex.Message}");
                    Console.WriteLine("Make sure OrchestratorService is running!");
                    continue; // Continue to next iteration instead of crashing
                }


                Console.WriteLine("next");
                Console.ReadKey();
            }
        }
    }
}
