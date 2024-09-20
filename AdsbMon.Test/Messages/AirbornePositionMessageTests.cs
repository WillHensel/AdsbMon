using AdsbMon.Core.Models;
using AdsbMon.Core.Models.Messages;

namespace AdsbMon.Test.Decoder;

[TestFixture]
public class AirbornePositionMessageTests
{

    [Test]
    public void FromHex_EvenFrameTest()
    {
        var message = AirbornePositionMessage.FromHex("58C382D690C8AC") as AirbornePositionMessage;

        Assert.That(message.TypeCode, Is.EqualTo(11));
        Assert.That(message.SurveillanceStatus, Is.EqualTo(SurveillanceStatus.NoCondition));
        Assert.That(message.Altitude, Is.EqualTo(38000));
        Assert.That(message.CprFormat, Is.EqualTo(CprFormat.EvenFrame));
        Assert.That(message.EncodedLatitude, Is.EqualTo(93000));
        Assert.That(message.EncodedLongitude, Is.EqualTo(51372));
    }
    
    [Test]
    public void FromHex_OddFrameTest()
    {
        var message = AirbornePositionMessage.FromHex("58C386435CC412") as AirbornePositionMessage;

        Assert.That(message.TypeCode, Is.EqualTo(11));
        Assert.That(message.SurveillanceStatus, Is.EqualTo(SurveillanceStatus.NoCondition));
        Assert.That(message.Altitude, Is.EqualTo(38000));
        Assert.That(message.CprFormat, Is.EqualTo(CprFormat.OddFrame));
        Assert.That(message.EncodedLatitude, Is.EqualTo(74158));
        Assert.That(message.EncodedLongitude, Is.EqualTo(50194));
    }
}