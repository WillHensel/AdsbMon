using AdsbMon.Core.Models;
using AdsbMon.Core.Models.Messages;

namespace AdsbMon.Test.Decoder;

[TestFixture]
public class AircraftIdentificationMessageTests
{
    [Test]
    public void FromHexTest()
    {
        var message = AircraftIdentificationMessage.FromHex("202CC371C32CE0") as AircraftIdentificationMessage;

        Assert.That(message.Category, Is.EqualTo(AircraftCategory.None));
        Assert.That(message.Callsign, Is.EqualTo("KLM1023 "));
    }
}