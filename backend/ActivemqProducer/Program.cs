using System;
using System.Net.Http;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace ActivemqProducer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create a Connection Factory.
            Uri connecturi = new Uri("activemq:tcp://localhost:61616");
            ConnectionFactory connectionFactory = new ConnectionFactory(connecturi);

            // Create a single Connection from the Connection Factory.
            IConnection connection = connectionFactory.CreateConnection();
            connection.Start();

            // Create a session from the Connection.
            ISession session = connection.CreateSession();

            // Use the session to target a queue.
            IDestination destination = session.GetQueue("QueueStomp");

            // Create a Producer targetting the selected queue.
            IMessageProducer producer = session.CreateProducer(destination);

            // You may configure everything to your needs, for instance:
            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
            ITextMessage message;

            string orchestBaseUrl = "http://localhost:8733/Design_Time_Addresses/OrchestratorService/OrchestratorService";
            HttpClient client = new HttpClient();


            //while (true)
            //{
            //    // Finally, to send messages:
            //    //message = session.CreateTextMessage("Hello Worldzrfzergezrgzregzrgzrgzrwaaaaaaaaaaaah");
            //    //producer.Send(message);
            //    //Console.WriteLine("Message sent, press Enter to send another or type 'exit' to quit.");
            //    //string input = Console.ReadLine();
            //    var requestUrl = $"{orchestBaseUrl}/meteo?coords";
            //    Task.Delay(15000);
            //    if (input?.ToLower() == "exit")
            //    {
            //        break;
            //    }
            //}

            // Don't forget to close your session and connection when finished.
            session.Close();
            connection.Close();
        }
    }
}
