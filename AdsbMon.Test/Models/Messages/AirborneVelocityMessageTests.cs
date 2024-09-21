using AdsbMon.Core.Models;
using AdsbMon.Core.Models.Messages;

namespace AdsbMon.Test.Models.Messages;

[TestFixture]
public class AirborneVelocityMessageTests
{
    [Test]
    public void FromHex_GroundSpeedTest()
    {
        var message = AirborneVelocityMessage.FromHex("99440994083817") as AirborneVelocityMessage;

        Assert.That(message!.VerticalRateSrc, Is.EqualTo(VerticalRateSource.Gnss));
        Assert.That(message.VerticalRate, Is.EqualTo(-832));
        
        var groundSpeedSubMessage = message.SubMessage as GroundSpeed;
        Assert.That(groundSpeedSubMessage!.Speed, Is.EqualTo(159.20));
        Assert.That(groundSpeedSubMessage.TrackAngle, Is.EqualTo(182.88));
    }

    [Test]
    public void FromHex_AirspeedTet()
    {
        var message = AirborneVelocityMessage.FromHex("9B06B6AF189400") as AirborneVelocityMessage;

        Assert.That(message!.VerticalRateSrc, Is.EqualTo(VerticalRateSource.Barometer));
        Assert.That(message.VerticalRate, Is.EqualTo(-2304));

        var airspeedSubMessage = message.SubMessage as Airspeed;
        Assert.That(airspeedSubMessage!.Speed, Is.EqualTo(375));
        Assert.That(airspeedSubMessage.SpeedType, Is.EqualTo(AirspeedType.True));
        Assert.That(airspeedSubMessage.MagneticHeading, Is.EqualTo(243.98));
        
        // 110000
        // 53000
        // 01011000110000111000001101011011011000001100111100001000
    }
}