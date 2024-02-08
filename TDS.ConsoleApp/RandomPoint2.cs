namespace TDS.ConsoleApp;


public class GeoCoordinate
{
    public double Latitude { get; }
    public double Longitude { get; }

    public GeoCoordinate(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public double GetDistanceTo(GeoCoordinate other)
    {
        double lat1 = Math.PI * Latitude / 180;
        double lon1 = Math.PI * Longitude / 180;
        double lat2 = Math.PI * other.Latitude / 180;
        double lon2 = Math.PI * other.Longitude / 180;

        double dlat = lat2 - lat1;
        double dlon = lon2 - lon1;

        double a = Math.Sin(dlat / 2) * Math.Sin(dlat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(dlon / 2) * Math.Sin(dlon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return 6371 * c;
    }
}

public static class RandomPoint2
{
    public static List<GeoCoordinate> GenerateRandomPoints(GeoCoordinate source, GeoCoordinate destination, int count)
    {
        // Calculate the distance between the source and destination points
        double distance = source.GetDistanceTo(destination);

        // Create a list to store the random points
        List<GeoCoordinate> randomPoints = new List<GeoCoordinate>();

        // Generate the specified number of random points between source and destination
        for (int i = 0; i < count; i++)
        {
            // Generate a random position between 0 and the total distance
            double randomPosition = distance * new Random().NextDouble();

            // Calculate the random point using spherical linear interpolation (slerp)
            randomPoints.Add(CalculateSlerp(source, destination, randomPosition / distance));
        }

        return randomPoints;
    }

    public static GeoCoordinate CalculateSlerp(GeoCoordinate a, GeoCoordinate b, double t)
    {
        // Normalize the input parameters
        double sinTheta = Math.Sin(a.GetDistanceTo(b));
        if (sinTheta == 0)
        {
            return a;
        }

        double theta = Math.Acos(sinTheta * Math.Sin(t) + Math.Cos(a.GetDistanceTo(b)) * Math.Cos(t));
        double sinOmega = Math.Sin(theta) / sinTheta;
        double cosOmega = Math.Cos(theta) - sinTheta * sinOmega * Math.Cos(a.GetDistanceTo(b));

        double x = a.Latitude * cosOmega + b.Latitude * sinOmega * Math.Sin(theta);
        double y = a.Longitude * cosOmega + b.Longitude * sinOmega * Math.Sin(theta);

        return new GeoCoordinate(x, y);
    }
}


