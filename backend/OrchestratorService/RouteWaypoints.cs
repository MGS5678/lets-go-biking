using System;
using System.Collections.Generic;

namespace OrchestratorService
{
    /// <summary>
    /// Représente un point d'intérêt sur l'itinéraire
    /// </summary>
    public class Waypoint
    {
        public double Lat { get; set; }
        public double Lng { get; set; }

        public Waypoint(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }
    }

    public class RouteWaypoints
    {
        private static RouteWaypoints _instance;
        private static readonly object _lock = new object();

        private List<Waypoint> _waypoints;
        private DateTime _lastUpdate;

        public static RouteWaypoints Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new RouteWaypoints();
                        }
                    }
                }
                return _instance;
            }
        }

        private RouteWaypoints()
        {
            _waypoints = new List<Waypoint>();
        }

        public void SetWaypoints(List<Coordinate> coords)
        {
            lock (_lock)
            {
                _waypoints.Clear();

                foreach (var coord in coords)
                {
                    _waypoints.Add(new Waypoint(coord.lat, coord.lng));
                }

                _lastUpdate = DateTime.Now;
                Console.WriteLine($"[RouteWaypoints] {_waypoints.Count} waypoints mis à jour");
            }
        }

        public List<Waypoint> GetWaypoints()
        {
            lock (_lock)
            {
                return new List<Waypoint>(_waypoints);
            }
        }
        public void Clear()
        {
            lock (_lock)
            {
                _waypoints.Clear();
                Console.WriteLine("[RouteWaypoints] Waypoints effacés");
            }
        }

        public DateTime LastUpdate => _lastUpdate;
    }
}
