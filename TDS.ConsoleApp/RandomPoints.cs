using System;
using System.Collections.Generic;


namespace TDS.ConsoleApp
{
  
    public class Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Point(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    public class RandomPointGenerator
    {
        private static readonly Random Random = new Random();

        public static List<Point> GenerateRandomPoints(Point source, Point destination, int count)
        {
            var points = new List<Point>();
            var distance = GetDistance(source, destination);

            for (int i = 0; i < count; i++)
            {
                var ratio = (double)i / count;
                var newLatitude = source.Latitude + ratio * (destination.Latitude - source.Latitude);
                var newLongitude = source.Longitude + ratio * (destination.Longitude - source.Longitude);

                // Add some randomness to the new point's position
                var randomLatitudeOffset = GetRandomOffset(distance);
                var randomLongitudeOffset = GetRandomOffset(distance);

                newLatitude += randomLatitudeOffset;
                newLongitude += randomLongitudeOffset;

                points.Add(new Point(newLatitude, newLongitude));
            }

            return points;
        }

        private static double GetDistance(Point point1, Point point2)
        {
            var lat1 = DegreesToRadians(point1.Latitude);
            var lon1 = DegreesToRadians(point1.Longitude);
            var lat2 = DegreesToRadians(point2.Latitude);
            var lon2 = DegreesToRadians(point2.Longitude);

            var dLat = lat2 - lat1;
            var dLon = lon2 - lon1;

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                     Math.Cos(lat1) * Math.Cos(lat2) *
                     Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return 6371 * c; // Distance in km
        }

        private static double GetRandomOffset(double distance)
        {
            var offset = Random.NextDouble() * distance;
            var direction = Random.Next(0, 2) == 0 ? -1 : 1;

            return offset * direction;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
