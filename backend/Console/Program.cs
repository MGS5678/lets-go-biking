using System;
using System.Diagnostics;
using System.Net.Http;

namespace ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();
            string contractName = string.Empty;


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
                    using (var response = client.GetAsync(requestUrl))
                    {
                        var result = response.Result;
                        Console.WriteLine($"Status Code: {result.StatusCode}");
                        var data = result.Content.ReadAsStringAsync().Result;
                        Debug.WriteLine("ConsoleClient - Program.cs - retrieved contract name: " + data + " for city: " + cityName);
                        if (result.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Response from contractsservice : {data}");
                            contractName = data.Trim('"');
                        }
                        else
                        {
                            Console.WriteLine($"Error Code : {result.StatusCode}");
                            Console.WriteLine($"Error Content : {data}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception : {ex.Message}");
                }

                requestUrl = $"{defaultBase}/stations?contract={contractName}";
                Console.WriteLine($"\n\nGetting stations for contract : {contractName}\n");
                Console.WriteLine($"Stations Request URL : {requestUrl}\n");
                try
                {
                    using (var response = client.GetAsync(requestUrl))
                    {
                        var result = response.Result;
                        Console.WriteLine($"Status Code: {result.StatusCode}");

                        var data = result.Content.ReadAsStringAsync().Result;

                        if (result.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Response from stationsservice : {data.Substring(0, 150)}");
                        }
                        else
                        {
                            Console.WriteLine($"Error Code : {result.StatusCode}");
                            Console.WriteLine($"Error Content : {data}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception : {ex.Message}");
                }



                Console.WriteLine("next");
                Console.ReadKey();
            }
        }
    }
}
