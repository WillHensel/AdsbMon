using System.Collections;
using AdsbMon.Core.Extensions;
using AdsbMon.Core.Utilities;

namespace AdsbMon.Core.Models.Messages;

public class AirbornePositionMessage : SubMessage
{
    // Omitted "Single antenna flag" and "Time" fields

    // 9-18: with barometric altitude
    // 20-22: with GNSS altitude
    public int TypeCode { get; }

    public SurveillanceStatus SurveillanceStatus { get; }

    public double Altitude { get; }

    public CprFormat CprFormat { get; }

    public int EncodedLatitude { get; }

    public int EncodedLongitude { get; }

    private AirbornePositionMessage(int typeCode, SurveillanceStatus surveillanceStatus,
        CprFormat cprFormat, int encodedLatitude, int encodedLongitude, BitArray encodedAltitude)
    {
        TypeCode = typeCode;
        SurveillanceStatus = surveillanceStatus;
        CprFormat = cprFormat;
        EncodedLatitude = encodedLatitude;
        EncodedLongitude = encodedLongitude;

        Altitude = GetDecodedAltitude(encodedAltitude);
    }

    public static SubMessage FromHex(string hex)
    {
        var bytes = Convert.FromHexString(hex);
        Array.Reverse(bytes);
        var bits = new BitArray(bytes);

        var encodedLongitude = bits.CopySlice(0, 17);
        var encodedLatitude = bits.CopySlice(17, 17);
        var cprFormat = bits.CopySlice(34, 1);
        var encodedAltitude = bits.CopySlice(36, 12);
        var surveillanceStatus = bits.CopySlice(49, 2);
        var typeCode = bits.CopySlice(51, 5);

        var message = new AirbornePositionMessage(
            typeCode.AsInt(),
            (SurveillanceStatus)surveillanceStatus.AsInt(),
            (CprFormat)cprFormat.AsInt(),
            encodedLatitude.AsInt(),
            encodedLongitude.AsInt(),
            encodedAltitude);

        return message;
    }

    private double GetDecodedAltitude(BitArray altitudeBits)
    {
        // GNSS altitude is the real altitude in meters
        if (TypeCode is >= 20 and <= 22)
        {
            return altitudeBits.AsInt() * 3.281;
        }
        
        // Barometric altitude is encoded
        // When the q bit is 1, altitude encoded with 25 ft increment of the integer
        // when the q bit is removed minus 1000 ft.
        var q = altitudeBits.Get(4);
        var newBits = new BitArray(11);
        for (int i = 0; i < 12; i++)
        {
            if (i > 4)
                newBits.Set(i-1, altitudeBits.Get(i));
            else
                newBits.Set(i, altitudeBits.Get(i));
        }

        var altitude = newBits.AsInt();
        if (q)
        {
            return 25 * altitude - 1000;
        }
        else
        {
            // TODO gray code decoding with not example provided by the book
            return 0;
        }
    }
}