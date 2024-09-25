namespace AdsbMon.Web.Models;

public class AircraftViewModel
{
    public string Icao { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? GroundSpeed { get; set; }
    public double? GroundTrackAngle { get; set; }
}