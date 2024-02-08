using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TDS.Shared.Helpers;

namespace TDS.ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        double longitude = HexStringToLongitude("1E9C747C");
        double latitude = HexStringToLatitude("154711AC");

        string latHexString = LatitudeToHexString(latitude);
        string longHexString = LongitudeToHexString(longitude);


        double longitude1 = HexStringToLongitude("0f0ea850");
        double latitude1 = HexStringToLatitude("209a6900");

        string latHexString1 = LatitudeToHexString(latitude1);
        string longHexString1 = LongitudeToHexString(longitude1);


        Console.WriteLine(longitude);
        int imeiCount = 1500;
        GenerateRandomListIMEI(imeiCount);

        for (int i = 0; i < imeiCount; i++)
        {
            var randImei = new Random().Next(0, imeiCount);

            Task.Run(async () =>
            {
                SimulateTeltonika(ImieList[randImei]);

            });
        }






        ////Timer timer = new Timer(state =>
        ////{
        //SimulateTeltonika(ImieList[randImei]);
        ////}, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));

        // Keep the application running until the timer is complete
        Console.ReadLine();
    }


    public static string LongitudeToHexString(double longitude)
    {
        // Adjust for the fact that the longitude ranges from -180 to 180 degrees
        longitude = Math.Abs(longitude);
        longitude = longitude % 180;
        if (longitude >= 180)
        {
            longitude -= 180;
        }

        // Convert the longitude value to a big-endian signed integer
        long intValue = (long)(longitude * 1E7);
        byte[] bytes = BitConverter.GetBytes(intValue);
        Array.Reverse(bytes);

        // Convert the integer value to a hex string
        StringBuilder hexString = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            hexString.AppendFormat("{0:X2}", b);
        }

        return hexString.ToString().Substring(8);
    }

    public static string LatitudeToHexString(double latitude)
    {
        // Adjust for the fact that the latitude ranges from -90 to 90 degrees
        latitude = Math.Abs(latitude);
        latitude = latitude % 180;
        if (latitude >= 90)
        {
            latitude -= 180;
        }

        // Convert the latitude value to a big-endian signed integer
        long intValue = (long)(latitude * 1E7);
        byte[] bytes = BitConverter.GetBytes(intValue);
        Array.Reverse(bytes);

        // Convert the integer value to a hex string
        StringBuilder hexString = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            hexString.AppendFormat("{0:X2}", b);
        }

        return hexString.ToString().Substring(8);
    }


    public static double HexStringToLatitude(string hexString)
    {
        // Convert the hex string to a byte array
        byte[] bytes = new byte[hexString.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
        }

        // Treat the byte array as a big-endian signed integer
        int sign = bytes[0] & 0x80;
        int intValue = BitConverter.ToInt32(bytes.Reverse().ToArray(), 0);
        if (sign != 0)
        {
            intValue = ~intValue + 1;
        }

        // Convert the integer value to latitude in degrees
        double latitude = (double)intValue / 1E7;
        if (sign != 0)
        {
            latitude = -latitude;
        }

        // Adjust for the fact that the latitude ranges from -90 to 90 degrees
        latitude = latitude % 180;
        if (latitude >= 90)
        {
            latitude -= 180;
        }

        return latitude;
    }

    public static double HexStringToLongitude(string hexString)
    {
        // Convert the hex string to a byte array
        byte[] bytes = new byte[hexString.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
        }

        // Treat the byte array as a big-endian signed integer
        int sign = bytes[0] & 0x80;
        int intValue = BitConverter.ToInt32(bytes.Reverse().ToArray(), 0);
        if (sign != 0)
        {
            intValue = ~intValue + 1;
        }

        // Convert the integer value to longitude in degrees
        double longitude = (double)intValue / 1E7;
        if (sign != 0)
        {
            longitude = -longitude;
        }

        return longitude;
    }


    private static List<string> ImieList = new List<string>();

    private static long HexStringToLong(string hexString)
    {
        return Convert.ToInt64(hexString, 16);
    }

    private static string LongToHexString(long value)
    {
        return Convert.ToString(value, 16);
    }


    private static DateTime ConvertTimestampToDateTime(long timestamp)
    {
        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dt = dt.AddMilliseconds(timestamp);
        return dt;
    }

    public static long ConvertDatetimeToTimestamp(DateTime datetime)
    {
        // Convert the DateTime object to a Unix timestamp in milliseconds
        var timestamp = (long)(datetime - new DateTime(1970, 1, 1)).TotalMilliseconds;

        // Ensure the timestamp has the minimum length of 13 digits
        if (timestamp.ToString().Length < 13)
        {
            var padding = 13 - timestamp.ToString().Length;
            var zeros = new string('0', padding);
            return long.Parse(zeros + timestamp.ToString());
        }
        return timestamp;
    }


    private static string ConvertTimestampToHexString(long timestamp)
    {
        // Convert timestamp to a byte array
        byte[] bytes = BitConverter.GetBytes(timestamp);

        Array.Reverse(bytes);
        // Convert each byte to a two-digit hexadecimal string and concatenate them
        string hexString = BitConverter.ToString(bytes).Replace("-", "").ToUpper();

        return hexString;
    }

    private static long ConvertHexStringToTimestamp(string hexDateTime) =>
        HexUtil.ConvertHexStringToByteArray(hexDateTime).ToInt64();


    private static DateTime ConvertHexStringToDateTime(string hexDateTime) =>
        DateTime.UnixEpoch.AddMicroseconds(HexUtil.ConvertHexStringToByteArray(hexDateTime).ToInt64());

    private static string ConvertDateTimeToHexString(DateTime dateTime)
    {
        var timeStamp = ConvertDatetimeToTimestamp(dateTime);
        return ConvertTimestampToHexString(timeStamp);
    }

    private static string CreateIMEI(string first14Digits)
    {
        if (first14Digits.Length != 14)
            throw new ArgumentException("The input string must contain 14 digits");

        int sum = 0;
        for (int i = 0; i < first14Digits.Length; i++)
        {
            int digit = int.Parse(first14Digits[i].ToString());

            if (i % 2 == 0)
                digit = digit * 2;
            sum += digit > 9 ? digit / 10 + digit % 10 : digit;
        }

        int checkDigit = (10 - (sum % 10)) % 10;
        return first14Digits + checkDigit;
    }

    private static string GenerateRandomIMEI()
    {
        Random rand = new Random();
        string random14Digits = string.Join("", Enumerable.Repeat(0, 14).Select(i => rand.Next(0, 10).ToString()));
        return CreateIMEI(random14Digits);
    }

    private static void GenerateRandomListIMEI(int count)
    {
        var list = new List<string>();

        for (int i = 0; i < count; i++)
        {
            list.Add(GenerateRandomIMEI());
        }

        ImieList.AddRange(list);
    }

    private static async Task SimulateTeltonika(string imei)
    {
        TcpClient tcpClient = new TcpClient();
        //tcpClient.Connect("192.168.3.23", 9090);
        tcpClient.Connect(IPAddress.Loopback, 7002);
        NetworkStream networkStream = tcpClient.GetStream();




        //--------------------------Auth Packet
        //var imei = "359632107251230";

        var imeiHexString = "000F" + HexUtil.ConvertStringToHexString(imei);


        networkStream.Write(HexUtil.ConvertHexStringToByteArray(imeiHexString));
        Console.WriteLine($"[x] Send Login Request At {DateTime.Now}");

        // Add a delay between sending messages
        Thread.Sleep(1000);

        // Read the response from the server
        byte[] buffer = new byte[1024];
        int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
        string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        if (!string.IsNullOrEmpty(response))
        {
            Console.WriteLine($"[x] Receive Response At {DateTime.Now}");

            //Timer timer = new Timer(state =>
            //{
                var hexDateTime = ConvertDateTimeToHexString(DateTime.Now);


                //Thread.Sleep(1000);
                networkStream.Write(HexUtil.ConvertHexStringToByteArray(
                "00000000" // 4 zeroes 4 bytes
                +
                "000004D6" // data length 4 bytes
                +
                "08" // codec ID 1 bytes
                +
                "13" // number od data (1 record)
                +
                //"0000018A7E1AF310" // timestamp in milliseconds (137)
                // "0000013feb55ff74" // timestamp in milliseconds (137)
                hexDateTime
                +
                "00" + // priority


                 // GPS Element

                 "1E9C747C" + // longitude
                 "154711AC" + // latitude

                //longHex + // longitude
                //latHex + // latitude


                "04CB" + // altitude
                "0000" + // angle
                "08" +  // sattelites
                "0000" + // speed



                //IO Element
                "00" + // io element id
                "0C" + // 30 io elements in record(total)
                "05" + // 9 io elements,which length is 1 Byte
                "EF00F0001505C800450105B50008B60007422F54430F5544000002F10000A8E310085FFF8A000000018A7E1A7DE0001E9C747C154711AC04CB0000080000000C05EF00F0001505C800450105B50008B60007422F54430F5544000002F10000A8E310085FFF8A000000018A7E1A08B0001E9C747C154711AC04CB0000070000000C05EF00F0001505C800450105B50009B60007422F54430F5644000002F10000A8E310085FFF8A000000018A7E199380001E9C747C154711AC04CB0000070000000C05EF00F0001505C800450105B50009B60007422F51430F5644000002F10000A8E310085FFF8A000000018A7E191E50001E9C747C154711AC04CA0000070000000C05EF00F0001505C800450105B50009B60007422F54430F5644000002F10000A8E310085FFF8A000000018A7E18A920001E9C747C154711AC04CA0000070000000C05EF00F0001505C800450105B50009B60007422F4E430F5944000002F10000A8E310085FFF8A000000018A7E1833F0001E9C747C154711AC04CA0000070000000C05EF00F0001505C800450105B50009B60007422F57430F5844000002F10000A8E310085FFF8A000000018A7E17BEC0001E9C747C154711AC04CA0000070000000C05EF00F0001505C800450105B50009B60007422F59430F5A44000002F10000A8E310085FFF8A000000018A7E174990001E9C747C154711AC04E10000060000000C05EF00F0001505C800450105B50010B6000F422F51430F5A44000002F10000A8E310085FFF8A000000018A7E16D460001E9C747C154711AC04E20000060000000C05EF00F0001505C800450105B50010B6000F422F50430F5A44000002F10000A8E310085FFF8A000000018A7E165F30001E9C747C154711AC04E30000060000000C05EF00F0001505C800450105B50010B6000F422F56430F5A44000002F10000A8E310085FFF8A000000018A7E15EA00001E9C747C154711AC04E30000060000000C05EF00F0001505C800450105B50010B6000F422F59430F5944000002F10000A8E310085FFF8A000000018A7E1574D0001E9C747C154711AC04E30000060000000C05EF00F0001505C800450105B50010B6000F422F4E430F5A44000002F10000A8E310085FFF8A000000018A7E14FFA0001E9C747C154711AC04E40000060000000C05EF00F0001505C800450105B50010B6000F422F51430F5A44000002F10000A8E310085FFF8A000000018A7E148A70001E9C747C154711AC04E40000060000000C05EF00F0001505C800450105B50010B6000F422F4C430F5A44000002F10000A8E310085FFF8A000000018A7E141540001E9C747C154711AC04E40000060000000C05EF00F0001505C800450105B50010B6000F422F56430F5A44000002F10000A8E310085FFF8A000000018A7E13A010001E9C747C154711AC04E40000060000000C05EF00F0001505C800450105B50010B6000F422F50430F5E44000002F10000A8E310085FFF8A000000018A7E132AE0001E9C747C154711AC04E40000050000000C05EF00F0001505C800450105B50013B60012422F4F430F5E44000002F10000A8E310085FFF8A000000018A7E12B5B0001E9C747C154711AC00AA0132050000000C05EF00F0001505C800450105B50013B60012422F50430F6344000002F10000A8E310085FFF8A00130000203A"));

                Console.WriteLine($"[x] Send Location Packate At {DateTime.Now}");





            //}, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));

         
            

            networkStream.Close();
            tcpClient.Close();
        }

        await Task.CompletedTask;
    }


    //private static void SimulateTeltonika()
    //{
    //    TcpClient tcpClient = new TcpClient();
    //    //tcpClient.Connect("192.168.3.23", 9090);
    //     tcpClient.Connect(IPAddress.Loopback, 7002);
    //    NetworkStream networkStream = tcpClient.GetStream();

    //    networkStream.Write(HexUtil.ConvertHexStringToByteArray("000F333532383438303236333839393631"));
    //    Console.WriteLine($"[x] Send Login Request At {DateTime.Now}");

    //    // Add a delay between sending messages
    //    Thread.Sleep(1000);

    //    // Read the response from the server
    //    byte[] buffer = new byte[1024];
    //    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
    //    string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
    //    if (!string.IsNullOrEmpty(response))
    //    {
    //        Console.WriteLine($"[x] Receive Response At {DateTime.Now}");

    //        while (true)
    //        {

    //            Thread.Sleep(1000);

    //            //long longitude = HexStringToLong("1E9C747C");
    //            //long latitude = HexStringToLong("154711AC");


    //            //var longitudeHex = LongToHexString(longitude);
    //            //var latitudeHex = LongToHexString(latitude);

    //            //networkStream.Write(HexUtil.ConvertHexStringToByteArray("00000000000003E108120000011733FAD2C80000000000000000000000000000000000080301014501F00003B60000422F5343004002C700000000F100005852000000011733FAAB2C0000000000000000000000000000000000080301014501F00003B60000422F7A43004602C700000000F100005852000000011733FA83720000000000000000000000000000000000080301014501F00003B60000422F6E43003D02C700000000F100005852000000011733FA5BD60000000000000000000000000000000000080301014501F00003B60000422F7843004902C700000000F100005852000000011733FA343A0000000000000000000000000000000000080301014501F00003B60000422F6D43004D02C700000000F100000000000000011733FA0C940000000000000000000000000000000000080301014501F00003B60000422F7643004302C700000000F100000000000000011733F9E4EE0000000000000000000000000000000000080301014501F00003B60000422F7B43004202C700000000F100000000000000011733F9BD520000000000000000000000000000000000080301014501F00003B60000422F7E43004602C700000000F100000000000000011733F995990000000000000000000000000000000000080301014501F00003B60000422F7543004102C700000000F100005852000000011733F96DE80000000000000000000000000000000000080301014501F00003B60000422F6743003E02C700000000F100005852000000011733F9464C0000000000000000000000000000000000080301014501F00003B60000422F6E43004A02C700000000F100005852000000011733F91E9C0000000000000000000000000000000000080301014501F00003B60000422F6943004602C700000000F100000000000000011733F8F6F60000000000000000000000000000000000080301014501F00003B60000422F7643004602C700000000F100000000000000011733F8CF5A0000000000000000000000000000000000080301014501F00003B60000422F6443004902C700000000F100000000000000011733F8A7B40000000000000000000000000000000000080301014501F00003B60000422F7B43004102C700000000F100000000000000011733F8800E0000000000000000000000000000000000080301014501F00003B60000422F6F43005002C700000000F100005852000000011733F8586A0000000000000000000000000000000000080301014501F00003B60000422F4F43004002C700000000F100005852000000011733F830C20000000000000000000000000000000000080301014501F00003B60000422F7643004102C700000000F100005852001200004EC8"));

    //            networkStream.Write(HexUtil.ConvertHexStringToByteArray(
    //        "00000000" // 4 zeroes 4 bytes
    //        +
    //        "000004D6" // data length 4 bytes
    //        +
    //        "08" // codec ID 1 bytes
    //        +
    //        "13" // number od data (1 record)
    //        +
    //        "0000018A7E1AF310" // timestamp in milliseconds (137)
    //        +
    //        "00" + // priority


    //         // GPS Element

    //         "1E9C747C" + // longitude
    //         "154711AC" + // latitude

    //        //longHex + // longitude
    //        //latHex + // latitude


    //        "04CB" + // altitude
    //        "0000" + // angle
    //        "08" +  // sattelites
    //        "0000" + // speed



    //        //IO Element
    //        "00" + // io element id
    //        "0C" + // 30 io elements in record(total)
    //        "05" + // 9 io elements,which length is 1 Byte
    //        "EF00F0001505C800450105B50008B60007422F54430F5544000002F10000A8E310085FFF8A000000018A7E1A7DE0001E9C747C154711AC04CB0000080000000C05EF00F0001505C800450105B50008B60007422F54430F5544000002F10000A8E310085FFF8A000000018A7E1A08B0001E9C747C154711AC04CB0000070000000C05EF00F0001505C800450105B50009B60007422F54430F5644000002F10000A8E310085FFF8A000000018A7E199380001E9C747C154711AC04CB0000070000000C05EF00F0001505C800450105B50009B60007422F51430F5644000002F10000A8E310085FFF8A000000018A7E191E50001E9C747C154711AC04CA0000070000000C05EF00F0001505C800450105B50009B60007422F54430F5644000002F10000A8E310085FFF8A000000018A7E18A920001E9C747C154711AC04CA0000070000000C05EF00F0001505C800450105B50009B60007422F4E430F5944000002F10000A8E310085FFF8A000000018A7E1833F0001E9C747C154711AC04CA0000070000000C05EF00F0001505C800450105B50009B60007422F57430F5844000002F10000A8E310085FFF8A000000018A7E17BEC0001E9C747C154711AC04CA0000070000000C05EF00F0001505C800450105B50009B60007422F59430F5A44000002F10000A8E310085FFF8A000000018A7E174990001E9C747C154711AC04E10000060000000C05EF00F0001505C800450105B50010B6000F422F51430F5A44000002F10000A8E310085FFF8A000000018A7E16D460001E9C747C154711AC04E20000060000000C05EF00F0001505C800450105B50010B6000F422F50430F5A44000002F10000A8E310085FFF8A000000018A7E165F30001E9C747C154711AC04E30000060000000C05EF00F0001505C800450105B50010B6000F422F56430F5A44000002F10000A8E310085FFF8A000000018A7E15EA00001E9C747C154711AC04E30000060000000C05EF00F0001505C800450105B50010B6000F422F59430F5944000002F10000A8E310085FFF8A000000018A7E1574D0001E9C747C154711AC04E30000060000000C05EF00F0001505C800450105B50010B6000F422F4E430F5A44000002F10000A8E310085FFF8A000000018A7E14FFA0001E9C747C154711AC04E40000060000000C05EF00F0001505C800450105B50010B6000F422F51430F5A44000002F10000A8E310085FFF8A000000018A7E148A70001E9C747C154711AC04E40000060000000C05EF00F0001505C800450105B50010B6000F422F4C430F5A44000002F10000A8E310085FFF8A000000018A7E141540001E9C747C154711AC04E40000060000000C05EF00F0001505C800450105B50010B6000F422F56430F5A44000002F10000A8E310085FFF8A000000018A7E13A010001E9C747C154711AC04E40000060000000C05EF00F0001505C800450105B50010B6000F422F50430F5E44000002F10000A8E310085FFF8A000000018A7E132AE0001E9C747C154711AC04E40000050000000C05EF00F0001505C800450105B50013B60012422F4F430F5E44000002F10000A8E310085FFF8A000000018A7E12B5B0001E9C747C154711AC00AA0132050000000C05EF00F0001505C800450105B50013B60012422F50430F6344000002F10000A8E310085FFF8A00130000203A"));

    //            Console.WriteLine($"[x] Send Location Packate At {DateTime.Now}");


    //        }

    //        networkStream.Close();
    //        tcpClient.Close();
    //    }




    //    //TcpClient tcpClient = new();
    //    //tcpClient.Connect("192.168.3.23", 9090);
    //    //NetworkStream networkStream = tcpClient.GetStream();

    //    //Console.WriteLine("[x] Send Request");

    //    //networkStream.Write(HexUtil.ConvertHexStringToByteArray("000F333532383438303236333839393631"));
    //    //networkStream.Write(HexUtil.ConvertHexStringToByteArray("00000000000003E108120000011733FAD2C80000000000000000000000000000000000080301014501F00003B60000422F5343004002C700000000F100005852000000011733FAAB2C0000000000000000000000000000000000080301014501F00003B60000422F7A43004602C700000000F100005852000000011733FA83720000000000000000000000000000000000080301014501F00003B60000422F6E43003D02C700000000F100005852000000011733FA5BD60000000000000000000000000000000000080301014501F00003B60000422F7843004902C700000000F100005852000000011733FA343A0000000000000000000000000000000000080301014501F00003B60000422F6D43004D02C700000000F100000000000000011733FA0C940000000000000000000000000000000000080301014501F00003B60000422F7643004302C700000000F100000000000000011733F9E4EE0000000000000000000000000000000000080301014501F00003B60000422F7B43004202C700000000F100000000000000011733F9BD520000000000000000000000000000000000080301014501F00003B60000422F7E43004602C700000000F100000000000000011733F995990000000000000000000000000000000000080301014501F00003B60000422F7543004102C700000000F100005852000000011733F96DE80000000000000000000000000000000000080301014501F00003B60000422F6743003E02C700000000F100005852000000011733F9464C0000000000000000000000000000000000080301014501F00003B60000422F6E43004A02C700000000F100005852000000011733F91E9C0000000000000000000000000000000000080301014501F00003B60000422F6943004602C700000000F100000000000000011733F8F6F60000000000000000000000000000000000080301014501F00003B60000422F7643004602C700000000F100000000000000011733F8CF5A0000000000000000000000000000000000080301014501F00003B60000422F6443004902C700000000F100000000000000011733F8A7B40000000000000000000000000000000000080301014501F00003B60000422F7B43004102C700000000F100000000000000011733F8800E0000000000000000000000000000000000080301014501F00003B60000422F6F43005002C700000000F100005852000000011733F8586A0000000000000000000000000000000000080301014501F00003B60000422F4F43004002C700000000F100005852000000011733F830C20000000000000000000000000000000000080301014501F00003B60000422F7643004102C700000000F100005852001200004EC8"));

    //    //Console.ReadKey();
    //    //networkStream.Close();
    //    //tcpClient.Close();
    //}

    private static (double, double) GetRandomLocation()
    {
        Random rand = new Random();

        // Generate a random latitude between 46.7662 and 46.7781 degrees (the latitude range of the center of Cluj-Napoca)
        double lat = rand.NextDouble() * 0.0119 + 46.7662;

        // Generate a random longitude between 23.5896 and 23.6162 degrees (the longitude range of the center of Cluj-Napoca)
        double lon = rand.NextDouble() * 0.0266 + 23.5896;

        return (lat, lon);
    }

    private static (double, double) GetRandomLocationInIran()
    {
        Random rand = new Random();

        // Define the boundaries of Iran in latitude and longitude
        double minLatitude = 24.2704;
        double maxLatitude = 39.8282;
        double minLongitude = 44.3261;
        double maxLongitude = 62.3336;

        // Generate a random latitude within the latitude boundaries of Iran
        double lat = rand.NextDouble() * (maxLatitude - minLatitude) + minLatitude;

        // Generate a random longitude within the longitude boundaries of Iran
        double lon = rand.NextDouble() * (maxLongitude - minLongitude) + minLongitude;

        return (lat, lon);
    }






}