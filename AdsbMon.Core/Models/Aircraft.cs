using AdsbMon.Core.Models.Messages;

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
}