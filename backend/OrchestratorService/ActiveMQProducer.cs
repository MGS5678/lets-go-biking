using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OrchestratorService
{
    public class ActiveMQProducer
    {
        private static ActiveMQProducer _instance;
        private static readonly object _lock = new object();

        private IConnection _connection;
        private ISession _session;
        private IMessageProducer _producer;
        private Thread _notificationThread;
        private bool _isRunning;
        private readonly int _intervalMs = 15000;

        private readonly HttpClient _httpClient = new HttpClient();
        private readonly ProxyClient proxy;
        public static ActiveMQProducer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ActiveMQProducer();
                        }
                    }
                }
                return _instance;
            }
        }

        private ActiveMQProducer()
        {
            InitializeConnection();
            proxy = new ProxyClient(_httpClient);
        }

        private void InitializeConnection()
        {
            try
            {
                Uri connectUri = new Uri("activemq:tcp://localhost:61616");
                ConnectionFactory connectionFactory = new ConnectionFactory(connectUri);

                _connection = connectionFactory.CreateConnection();
                _connection.Start();

                _session = _connection.CreateSession();
                IDestination destination = _session.GetQueue("MeteoNotifications");

                _producer = _session.CreateProducer(destination);
                _producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

                Console.WriteLine("[ActiveMQ] Connexion établie sur MeteoNotifications");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ActiveMQ] Erreur de connexion: {ex.Message}");
            }
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _notificationThread = new Thread(NotificationLoop)
            {
                IsBackground = true,
                Name = "MeteoNotificationThread"
            };
            _notificationThread.Start();
            Console.WriteLine("[ActiveMQ] Thread de notifications démarré");
        }

        public void Stop()
        {
            _isRunning = false;
            _notificationThread?.Join(5000);
            Console.WriteLine("[ActiveMQ] Thread de notifications arrêté");
        }

        private void NotificationLoop()
        {
            while (_isRunning)
            {
                try
                {
                    var waypoints = RouteWaypoints.Instance.GetWaypoints();

                    if (waypoints != null && waypoints.Count > 0)
                    {
                        SendMeteoNotifications(waypoints);
                    }
                    else
                    {
                        Console.WriteLine("[ActiveMQ] Aucun waypoint actif, notification ignorée");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ActiveMQ] Erreur dans la boucle: {ex.Message}");
                }

                Thread.Sleep(_intervalMs);
            }
        }

        private async void SendMeteoNotifications(System.Collections.Generic.List<Waypoint> waypoints)
        {
            try
            {
                List<object> meteoResults = new List<object>();

                foreach (var waypoint in waypoints)
                {
                    string coords = $"{waypoint.Lat.ToString().Replace(',', '.')},{waypoint.Lng.ToString().Replace(',', '.')}";
                    Debug.WriteLine("#############" + coords);
                    string meteoJson = await proxy.GetMeteo(coords);

                    meteoResults.Add(new
                    {
                        waypoint = waypoint,
                        meteo = JsonConvert.DeserializeObject(meteoJson)
                    });
                }

                var notification = new
                {
                    timestamp = DateTime.Now,
                    type = "meteo_update",
                    data = meteoResults
                };

                string json = JsonConvert.SerializeObject(notification);
                ITextMessage message = _session.CreateTextMessage(json);
                _producer.Send(message);

                Console.WriteLine($"[ActiveMQ] Notification envoyée: {waypoints.Count} waypoints");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ActiveMQ] Erreur d'envoi: {ex.Message}");
            }
        }


        public void Dispose()
        {
            Stop();
            _producer?.Close();
            _session?.Close();
            _connection?.Close();
        }
    }
}
