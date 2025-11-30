using System;
using System.ServiceModel;

namespace OrchestratorService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Type serviceType = typeof(OrchestratorService);

            using (ServiceHost host = new ServiceHost(serviceType))
            {
                try
                {
                    host.Open();

                    Console.WriteLine("Press enter to stop the service...");
                    Console.ReadLine();

                    host.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Service failed to start.");
                    Console.WriteLine(ex);

                    try { host.Abort(); } catch { }

                    Console.WriteLine("Press enter to exit...");
                    Console.ReadLine();
                }
            }
        }
    }
}
