using AdsbMon.Core.Models.Messages;
using AdsbMon.Core.Utilities;

namespace AdsbMon.Core.Models;

public class Aircraft
{
    public string Icao { get; private set; }
    public AircraftCategory? Category { get; private set; }
    public string? Callsign { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public double? GroundSpeed { get; private set; }
    public int? Airspeed { get; private set; }
    public int? VerticalRate { get; private set; }
    public bool IsAloft { get; private set; } = false;
    public double? MagneticHeading { get; private set; }
    public double? GroundTrackAngle { get; private set; }
    public AirbornePositionMessage? LastPositionMessage { get; private set; }

    private Aircraft(string icao)
    {
        Icao = icao;
    }

    public static Aircraft FromMessage(Message message)
    {
        var newAircraft = new Aircraft(message.Icao);

        newAircraft.UpdateWithMessage(message);

        return newAircraft;
    }

    public void UpdateWithMessage(Message message)
    {
        switch (message.SubMessage)
        {
            case AirbornePositionMessage msg:
                UpdateWithMessage(msg);
                break;
            case AirborneVelocityMessage msg:
                UpdateWithMessage(msg);
                break;
            case AircraftIdentificationMessage msg:
                UpdateWithMessage(msg);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void UpdateWithMessage(AirbornePositionMessage msg)
    {
        if (Latitude != null && Longitude != null)
        {
            UpdateCoordinatesUsingLocallyUnambiguousPositioning(msg);
        }
        else
        {
            if (LastPositionMessage == null || LastPositionMessage.CprFormat == msg.CprFormat)
            {
                LastPositionMessage = msg;
                return;
            }
            UpdateCoordinatesUsingGloballyUnambiguousPositioning(msg);
        }
    }

    private void UpdateWithMessage(AirborneVelocityMessage msg)
    {
        VerticalRate = msg.VerticalRate;

        switch (msg.SubMessage)
        {
            case GroundSpeed ground:
                GroundSpeed = ground.Speed;
                GroundTrackAngle = ground.TrackAngle;
                break;
            case Airspeed air:
                Airspeed = air.Speed;
                MagneticHeading = air.MagneticHeading;
                break;
        }
    }

    private void UpdateWithMessage(AircraftIdentificationMessage msg)
    {
        Category = msg.Category;
        Callsign = msg.Callsign;
    }

    private void UpdateCoordinatesUsingLocallyUnambiguousPositioning(AirbornePositionMessage msg)
    {
        if (Latitude == null || Longitude == null)
        {
            return;
        }
        
        var latCpr = msg.EncodedLatitude / Math.Pow(2, 17);
        var lonCpr = msg.EncodedLongitude / Math.Pow(2, 17);

        var latZoneSize = 360.0 / (4 * 15 - (int)msg.CprFormat);
        var latZone = Math.Floor(Latitude!.Value / latZoneSize) +
                      Math.Floor(
                          DecodingUtilities.FlooredMod(Latitude!.Value, latZoneSize) / 
                          latZoneSize - latCpr + 1 / 2.0);
        var latitude = latZoneSize * (latZone + latCpr);
        
        var nlLat = CalcLongitudeZoneNumber(latitude);
        var lonZoneSize = 360.0 / Math.Max(nlLat - (int)msg.CprFormat, 1);
        var lonZone = Math.Floor(Longitude!.Value / lonZoneSize) +
                      Math.Floor(
                          DecodingUtilities.FlooredMod(Longitude!.Value, lonZoneSize) / 
                          lonZoneSize - lonCpr + 1 / 2.0);
        var longitude = lonZoneSize * (lonZone + lonCpr);

        Latitude = Math.Round(latitude, 4);
        Longitude = Math.Round(longitude, 4);
        LastPositionMessage = msg;
    }

    private void UpdateCoordinatesUsingGloballyUnambiguousPositioning(AirbornePositionMessage msg)
    {
        if (LastPositionMessage == null)
        {
            return;
        }

        AirbornePositionMessage msgEven;
        AirbornePositionMessage msgOdd;
        CprFormat mostRecent;

        if (msg.CprFormat == CprFormat.EvenFrame)
        {
            msgEven = msg;
            msgOdd = LastPositionMessage;
            mostRecent = CprFormat.EvenFrame;
        }
        else
        {
            msgEven = LastPositionMessage;
            msgOdd = msg;
            mostRecent = CprFormat.OddFrame;
        }

        var latCprEven = msgEven.EncodedLatitude / Math.Pow(2, 17);
        var lonCprEven = msgEven.EncodedLongitude / Math.Pow(2, 17);
        var latCprOdd = msgOdd.EncodedLatitude / Math.Pow(2, 17);
        var lonCprOdd = msgOdd.EncodedLongitude / Math.Pow(2, 17);

        var latitude = CalcLatitude(latCprEven, latCprOdd, mostRecent);
        var longitude = CalcLongitude(lonCprEven, lonCprOdd, latitude, mostRecent);

        Latitude = Math.Round(latitude, 4);
        Longitude = Math.Round(longitude, 4);
        LastPositionMessage = msg;
    }

    private double CalcLatitude(double latEven, double latOdd, CprFormat mostRecentFormat)
    {
        var latitudeIndex = Math.Floor(59 * latEven - 60 * latOdd + 1 / 2.0);
        var latitudeEven = 360.0 / (4 * 15) * (DecodingUtilities.FlooredMod(latitudeIndex, 60) + latEven);
        var latitudeOdd = 360.0 / (4 * 15 - 1) * (DecodingUtilities.FlooredMod(latitudeIndex, 59) + latOdd);

        latitudeEven = NormalizeLatitude(latitudeEven);
        latitudeOdd = NormalizeLatitude(latitudeOdd);

        var nlLatEven = CalcLongitudeZoneNumber(latitudeEven);
        var nlLatOdd = CalcLongitudeZoneNumber(latitudeOdd);

        if (nlLatEven != nlLatOdd)
        {
            return 0.0;
        }

        return mostRecentFormat switch
        {
            CprFormat.EvenFrame => latitudeEven,
            CprFormat.OddFrame => latitudeOdd,
            _ => 0.0
        };
    }

    private double CalcLongitude(double lonEven, double lonOdd, double latitude, CprFormat mostRecentFormat)
    {
        var nlLat = CalcLongitudeZoneNumber(latitude);
        var m = Math.Floor(lonEven * (nlLat - 1) - lonOdd * nlLat + 1 / 2.0);

        var n = mostRecentFormat switch
        {
            CprFormat.EvenFrame => Math.Max(nlLat, 1),
            CprFormat.OddFrame => Math.Max(nlLat - 1, 1),
            _ => nlLat
        };

        var lonCpr = mostRecentFormat switch
        {
            CprFormat.EvenFrame => lonEven,
            CprFormat.OddFrame => lonOdd,
            _ => lonEven
        };

        var longitude = 360.0 / n * (DecodingUtilities.FlooredMod(m, n) + lonCpr);

        return NormalizeLongitude(longitude);
    }

    private int CalcLongitudeZoneNumber(double latitude)
    {
        var xNumerator = 1 - Math.Cos(Math.PI / 30);
        var xDenominator = Math.Pow(Math.Cos(Math.PI / 180 * latitude), 2);
        var x = 1 - xNumerator / xDenominator;

        return (int)Math.Floor(2 * Math.PI / Math.Acos(x));
    }

    private double NormalizeLatitude(double latitude)
    {
        return latitude switch
        {
            >= 270 => latitude - 360,
            _ => latitude
        };
    }

    private double NormalizeLongitude(double longitude)
    {
        return longitude switch
        {
            >= 180 => longitude - 360,
            _ => longitude
        };
    }
}