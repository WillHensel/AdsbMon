using AdsbMon.Core.Models;
using AdsbMon.Core.Models.Messages;

namespace AdsbMon.Test.Models;

[TestFixture]
public class AircraftTests
{

    [Test]
    public void UpdateWithMessage_AirbornePositionTest()
    {
        var aircraft = Aircraft.FromMessage(Message.FromHex("8D40621D58C382D690C8AC2863A7"));
    }
    
    [Test]
    public void UpdateWithMessage_AirborneVelocityTest()
    {
        var aircraftType1 = Aircraft.FromMessage(Message.FromHex("8D485020994409940838175B284F"));

        Assert.That(aircraftType1.Icao, Is.EqualTo("485020"));
        Assert.That(aircraftType1.VerticalRate, Is.EqualTo(-832));
        Assert.That(aircraftType1.GroundSpeed, Is.EqualTo(159.20));
        Assert.That(aircraftType1.GroundTrackAngle, Is.EqualTo(182.88));

        var aircraftType3 = Aircraft.FromMessage(Message.FromHex("8DA05F219B06B6AF189400CBC33F"));

        Assert.That(aircraftType3.Icao, Is.EqualTo("A05F21"));
        Assert.That(aircraftType3.VerticalRate, Is.EqualTo(-2304));
        Assert.That(aircraftType3.Airspeed, Is.EqualTo(375));
        Assert.That(aircraftType3.MagneticHeading, Is.EqualTo(243.98));
    }

    [Test]
    public void UpdateWithMessage_AircraftIdentificationTest()
    {
        var aircraft = Aircraft.FromMessage(Message.FromHex("8D4840D6202CC371C32CE0576098"));

        Assert.That(aircraft.Category, Is.EqualTo(AircraftCategory.None));
        Assert.That(aircraft.Callsign, Is.EqualTo("KLM1023 "));
    }
}