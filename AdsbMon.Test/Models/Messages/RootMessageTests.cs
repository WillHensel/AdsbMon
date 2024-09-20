using AdsbMon.Core.Models.Messages;

namespace AdsbMon.Test.Models.Messages;

[TestFixture]
public class RootMessageTests
{

    [Test]
    public void FromHexTest()
    {
        var message = Message.FromHex("8D4840D6202CC371C32CE0576098");

        Assert.That(message.DownLinkFormat, Is.EqualTo(17));
        Assert.That(message.Icao, Is.EqualTo("4840D6"));
    }
}