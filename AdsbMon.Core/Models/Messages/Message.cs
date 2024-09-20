using System.Collections;
using AdsbMon.Core.Extensions;
using AdsbMon.Core.Utilities;

namespace AdsbMon.Core.Models.Messages;

public class Message
{
    // Should always be 17
    public int DownLinkFormat { get; set; }

    public string Icao { get; set; } = string.Empty;

    public SubMessage SubMessage { get; set; } = null!;


    public static Message FromHex(string hex)
    {
        hex = hex.TrimStart('*').TrimEnd('\n').TrimEnd(';');

        if (hex.Length < 28)
        {
            throw new Exception("Message hex is the incorrect length");
        }

        var dfcaSub = hex.Substring(0, 2);
        var dfcaBytes = Convert.FromHexString(dfcaSub);
        var dfca = new BitArray(dfcaBytes);
        
        var icao = hex.Substring(2, 6);
        var message = hex.Substring(8, 14);

        var df = dfca.CopySlice(3, 5);

        var typeCode = GetTypeCodeFromSubMessage(message);

        return new Message()
        {
            DownLinkFormat = df.AsInt(),
            Icao = icao,
            SubMessage = DecodeSubMessage(message, typeCode)
        };
    }

    private static int GetTypeCodeFromSubMessage(string subMessage)
    {
        var bits = new BitArray(Convert.FromHexString(subMessage.Substring(0, 2)));
        var typeBits = bits.CopySlice(0, 5);
        var typeCode = typeBits.AsInt();

        return typeCode;
    }

    private static SubMessage DecodeSubMessage(string subMessage, int typeCode)
    {
        SubMessage decoded;
        
        switch (typeCode)
        {
            case >= 1 and <= 4:
                decoded = AircraftIdentificationMessage.FromHex(subMessage);
                break;
            default:
                decoded = AircraftIdentificationMessage.FromHex(subMessage);
                break;
        }

        return decoded;
    }
}