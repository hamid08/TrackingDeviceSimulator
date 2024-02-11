using System;
using System.Collections.Generic;



namespace RandomPathGenerator;

public static class RouteGenerator {

    public static List<LocationsWithImie> GenerateRouteForDevice(List<string> imeis)
    {
        var result = new List<LocationsWithImie>();
        foreach (var imei in imeis)
        {
            var sourceLocation = GetRandomLocationInIran();
            var distentionLocation = GetRandomLocationInRadius(sourceLocation, new Random().Next(2500, 55000));

            var dis =GetDistance(sourceLocation, distentionLocation);
            int roundedValue = (int)Math.Floor(dis);
            var temp = $"{roundedValue}";

            if (Convert.ToInt32($"{dis}".Split(".")[0]) > 0)
            {
                for (int i = 0; i < roundedValue.ToString().Length; i++)
                {
                    temp += "0";
                }

            }
            else
            {
                temp = "500";
            }

            var newRoundedValue = Convert.ToInt32(temp);

            Location london = new Location(sourceLocation.Latitude, sourceLocation.Longitude);
            Location newYork = new Location(distentionLocation.Latitude, distentionLocation.Longitude);
            int count = newRoundedValue;

            List<Location> routeLocations = GetLocationsInRoute(london, newYork, count);

            result.Add(new LocationsWithImie { Locations = routeLocations ,Imei = imei});
        }

        return result;
    }



    public static double GetDistance(Location location1, Location location2)
    {
        double R = 6371; // Radius of the Earth in kilometers

        double lat1 = location1.Latitude * Math.PI / 180;
        double lon1 = location1.Longitude * Math.PI / 180;
        double lat2 = location2.Latitude * Math.PI / 180;
        double lon2 = location2.Longitude * Math.PI / 180;

        double dLat = lat2 - lat1;
        double dLon = lon2 - lon1;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    public static Location GetRandomLocationInRadius(Location location, int radius)
    {
        Random random = new Random();

        // Convert radius from meters to degrees
        double radiusInDegrees = radius / 111000d;

        // Generate random point within a square of side length 2*radius
        double x = random.NextDouble() * radiusInDegrees * 2 - radiusInDegrees;
        double y = random.NextDouble() * radiusInDegrees * 2 - radiusInDegrees;

        // Adjust the point to be within a circle of radius radius
        double distanceFromCenter = Math.Sqrt(x * x + y * y);
        if (distanceFromCenter > radiusInDegrees)
        {
            double factor = radiusInDegrees / distanceFromCenter;
            x *= factor;
            y *= factor;
        }

        // Adjust for the shrinking of the east-west distances
        double new_x = x / Math.Cos(location.Latitude * Math.PI / 180);

        // Return the new location
        return new Location(location.Latitude + y, location.Longitude + new_x);
    }

    public static List<Location> GetLocationsInRoute(Location start, Location end, int count)
    {
        List<Location> routeLocations = new List<Location>();
        double distance = GetDistance(start, end);
        double step = distance / (count - 1);

        Random random = new Random();


        for (int i = 0; i < count; i++)
        {
            double progress = (double)i / (count - 1);
            double latitude = start.Latitude + progress * (end.Latitude - start.Latitude);
            double longitude = start.Longitude + progress * (end.Longitude - start.Longitude);

            // Add some randomness to the location
            latitude += random.NextDouble() * 0.0001 - 0.0005;
            longitude += random.NextDouble() * 0.0001 - 0.0005;

            routeLocations.Add(new Location(latitude, longitude));
        }

        return routeLocations;
    }

    private static (double, double) GetRandomLocation()
    {
        Random rand = new Random();

        // Generate a random latitude between 46.7662 and 46.7781 degrees (the latitude range of the center of Cluj-Napoca)
        double lat = rand.NextDouble() * 0.0119 + 46.7662;

        // Generate a random longitude between 23.5896 and 23.6162 degrees (the longitude range of the center of Cluj-Napoca)
        double lon = rand.NextDouble() * 0.0266 + 23.5896;

        return (lat, lon);
    }

    private static Location GetRandomLocationInIran()
    {
        Random rand = new Random();

        // Define the boundaries of Iran in latitude and longitude
        double minLatitude = 24.2704;
        double maxLatitude = 39.8282;
        double minLongitude = 44.3261;
        double maxLongitude = 62.3336;

        // Generate a random latitude within the boundaries of Iran
        double lat = rand.NextDouble() * (maxLatitude - minLatitude) + minLatitude;

        // Generate a random longitude within the boundaries of Iran
        double lon = rand.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

        return new Location(lat, lon);
    }


}

public class LocationsWithImie
{
    public string Imei { get; set; }
    public List<Location> Locations { get; set; }
}

public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public Location(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }


}






















//public class Location
//{
//    public double Latitude { get; set; }
//    public double Longitude { get; set; }

//    public Location(double latitude, double longitude)
//    {
//        Latitude = latitude;
//        Longitude = longitude;
//    }

//    public static double HaversineDistance(Location location1, Location location2)
//    {
//        double R = 6371; // Radius of the Earth in kilometers
//        double lat1 = location1.Latitude * Math.PI / 180;
//        double lon1 = location1.Longitude * Math.PI / 180;
//        double lat2 = location2.Latitude * Math.PI / 180;
//        double lon2 = location2.Longitude * Math.PI / 180;

//        double dLat = lat2 - lat1;
//        double dLon = lon2 - lon1;

//        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
//        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

//        return R * c;
//    }

//    public static Location Midpoint(Location location1, Location location2)
//    {
//        double lat1 = location1.Latitude * Math.PI / 180;
//        double lon1 = location1.Longitude * Math.PI / 180;
//        double lat2 = location2.Latitude * Math.PI / 180;
//        double lon2 = location2.Longitude * Math.PI / 180;

//        double Bx = Math.Cos(lat2) * Math.Cos(lon2 - lon1);
//        double By = Math.Cos(lat2) * Math.Sin(lon2 - lon1);
//        double lat3 = Math.Atan2(Math.Sin(lat1) + Math.Sin(lat2), Math.Sqrt((Math.Cos(lat1) + Bx) * (Math.Cos(lat1) + Bx) + By * By));
//        double lon3 = lon1 + Math.Atan2(By, Math.Cos(lat1) + Bx);

//        return new Location(lat3 * 180 / Math.PI, lon3 * 180 / Math.PI);
//    }

//    public static double GetDistance(Location location1, Location location2)
//    {
//        double R = 6371; // Radius of the Earth in kilometers

//        double lat1 = location1.Latitude * Math.PI / 180;
//        double lon1 = location1.Longitude * Math.PI / 180;
//        double lat2 = location2.Latitude * Math.PI / 180;
//        double lon2 = location2.Longitude * Math.PI / 180;

//        double dLat = lat2 - lat1;
//        double dLon = lon2 - lon1;

//        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
//        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

//        return R * c;
//    }

//    public static List<Location> GetLocationsInRoute(Location start, Location end, int count)
//    {
//        List<Location> routeLocations = new List<Location>();
//        double distance = Location.GetDistance(start, end);
//        double step = distance / (count - 1);

//        Location current = start;
//        Location next = current;
//        for (int i = 0; i < count; i++)
//        {
//            routeLocations.Add(current);
//            if (i < count - 1 && i > 0)
//            {
//                next = Midpoint(current, end);
//                double remainingDist = distance - step * i;
//                double distToNext = Location.GetDistance(current, next);
//                if (remainingDist < distToNext)
//                {
//                    next = new Location(current.Latitude + remainingDist * (end.Latitude - current.Latitude) / distToNext,
//                                       current.Longitude + remainingDist * (end.Longitude - current.Longitude) / distToNext);
//                }
//                current = next;
//            }
//        }

//        return routeLocations;
//    }


//}



















//namespace RandomPathGenerator
//{
//    public class Location
//    {
//        public double Latitude { get; set; }
//        public double Longitude { get; set; }

//        public Location(double latitude, double longitude)
//        {
//            Latitude = latitude;
//            Longitude = longitude;
//        }

//        public static List<Location> GetRandomLocationsInRadius(Location location, int radius, int count)
//        {

//            List<Location> randomLocations = new List<Location>();

//            for (int i = 0; i < count; i++)
//            {
//                randomLocations.Add(GetRandomLocationInRadius(location, radius));
//            }

//            return randomLocations;
//        }

//        public static Location GetRandomLocationInRadius(Location location, int radius)
//        {
//            Random random = new Random();

//            // Convert radius from meters to degrees
//            double radiusInDegrees = radius / 111000d;

//            // Generate random point within a square of side length 2*radius
//            double x = random.NextDouble() * radiusInDegrees * 2 - radiusInDegrees;
//            double y = random.NextDouble() * radiusInDegrees * 2 - radiusInDegrees;

//            // Adjust the point to be within a circle of radius radius
//            double distanceFromCenter = Math.Sqrt(x * x + y * y);
//            if (distanceFromCenter > radiusInDegrees)
//            {
//                double factor = radiusInDegrees / distanceFromCenter;
//                x *= factor;
//                y *= factor;
//            }

//            // Adjust for the shrinking of the east-west distances
//            double new_x = x / Math.Cos(location.Latitude * Math.PI / 180);

//            // Return the new location
//            return new Location(location.Latitude + y, location.Longitude + new_x);
//        }
//    }
//}




//namespace RandomPathGenerator
//{
//    public class Location
//    {
//        public double Latitude { get; set; }
//        public double Longitude { get; set; }

//        public Location(double latitude, double longitude)
//        {
//            Latitude = latitude;
//            Longitude = longitude;
//        }

//        public static Location GetRandomLocationInRadius(Location location, int radius)
//        {
//            Random random = new Random();

//            // Convert radius from meters to degrees
//            double radiusInDegrees = radius / 111000d;

//            // Generate random point within a square of side length 2*radius
//            double x = random.NextDouble() * radiusInDegrees * 2 - radiusInDegrees;
//            double y = random.NextDouble() * radiusInDegrees * 2 - radiusInDegrees;

//            // Adjust the point to be within a circle of radius radius
//            double distanceFromCenter = Math.Sqrt(x * x + y * y);
//            if (distanceFromCenter > radiusInDegrees)
//            {
//                double factor = radiusInDegrees / distanceFromCenter;
//                x *= factor;
//                y *= factor;
//            }

//            // Adjust for the shrinking of the east-west distances
//            double new_x = x / Math.Cos(location.Latitude * Math.PI / 180);

//            // Return the new location
//            return new Location(location.Latitude + y, location.Longitude + new_x);
//        }
//    }
//}