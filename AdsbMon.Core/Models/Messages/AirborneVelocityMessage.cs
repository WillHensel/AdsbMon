using System.Collections;
using System.ComponentModel;
using AdsbMon.Core.Extensions;
using AdsbMon.Core.Utilities;

namespace AdsbMon.Core.Models.Messages;

public class AirborneVelocityMessage : SubMessage
{
    // Omitted GNSS and baro differences fields

    public int IntentChangeFlag { get; }
    public int IfrCapabilityFlag { get; }
    public int NavUncertainty { get; }
    public VerticalRateSource VerticalRateSrc { get; }
    public int VerticalRate { get; }
    public VelocitySubMessage SubMessage { get; }

    private AirborneVelocityMessage(int intentChangeFlag, int ifrCapabilityFlag, int navUncertainty,
        VerticalRateSource verticalRateSrc, int verticalRate, VelocitySubMessage subMessage)
    {
        IntentChangeFlag = intentChangeFlag;
        IfrCapabilityFlag = ifrCapabilityFlag;
        NavUncertainty = navUncertainty;
        VerticalRateSrc = verticalRateSrc;
        VerticalRate = verticalRate;
        SubMessage = subMessage;
    }

    public static SubMessage FromHex(string hex)
    {
        var bytes = Convert.FromHexString(hex);
        Array.Reverse(bytes);
        var bits = new BitArray(bytes);

        // var gnssBaroDiff = bits.CopySlice(0, 7);
        // var gnssBaroDiffSign = bits.CopySlice(7, 1);
        var verticalRateBits = bits.CopySlice(10, 9);
        var verticalRateSign = bits.CopySlice(19, 1);
        var verticalRateSource = bits.CopySlice(20, 1);
        var subTypeBits = bits.CopySlice(21, 22);
        var navUncertainty = bits.CopySlice(43, 3);
        var ifrCapability = bits.CopySlice(46, 1);
        var intentChange = bits.CopySlice(47, 1);
        var subTypeCode = bits.CopySlice(48, 3);
        // var typeCode = bits.CopySlice(51, 5);

        int verticalRate = verticalRateBits.AsInt();
        if (verticalRateSign.AsInt() == 0)
        {
            verticalRate = 64 * (verticalRate - 1);
        }
        else
        {
            verticalRate = -64 * (verticalRate - 1);
        }

        VelocitySubMessage subMessage;
        switch (subTypeCode.AsInt())
        {
            case 1 or 2:
                subMessage = GroundSpeed.FromBits(subTypeBits, subTypeCode.AsInt());
                break;
            case 3 or 4:
                subMessage = Airspeed.FromBits(subTypeBits, subTypeCode.AsInt());
                break;
            default:
                subMessage = GroundSpeed.FromBits(subTypeBits, subTypeCode.AsInt());
                break;
        }

        return new AirborneVelocityMessage(intentChange.AsInt(), ifrCapability.AsInt(), navUncertainty.AsInt(),
            (VerticalRateSource)verticalRateSource.AsInt(), verticalRate, subMessage);
    }
}

public interface VelocitySubMessage
{
    public static abstract VelocitySubMessage FromBits(BitArray bits, int subType);
}

public class GroundSpeed : VelocitySubMessage
{
    public double Speed { get; }
    public double TrackAngle { get; }

    private GroundSpeed(double speed, double trackAngle)
    {
        Speed = speed;
        TrackAngle = trackAngle;
    }
    
    public static VelocitySubMessage FromBits(BitArray bits, int subType)
    {
        var northSouthVelocityComp = bits.CopySlice(0, 10).AsInt();
        var dirNorthSouth = bits.CopySlice(10, 1).AsInt();
        var eastWestVelocityComp = bits.CopySlice(11, 10).AsInt();
        var dirEastWest = bits.CopySlice(21, 1).AsInt();

        var vx = 0;
        var vy = 0;
        
        vx = dirEastWest switch
        {
            0 => eastWestVelocityComp - 1,
            1 => -1 * (eastWestVelocityComp - 1),
            _ => throw new ArgumentOutOfRangeException()
        };
        vy = dirNorthSouth switch
        {
            0 => northSouthVelocityComp - 1,
            1 => -1 * (northSouthVelocityComp - 1),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (subType == 2)
        {
            vx *= 4;
            vy *= 4;
        }

        var speed = Math.Sqrt(Math.Pow(vx, 2) + Math.Pow(vy, 2));

        var trackAngle = Math.Atan2(vx, vy) * 360 / (2 * Math.PI);
        trackAngle = DecodingUtilities.FlooredMod(trackAngle, 360);

        return new GroundSpeed(Math.Round(speed, 2), Math.Round(trackAngle, 2));
    }
}

public class Airspeed : VelocitySubMessage
{
    public int Speed { get; }
    public AirspeedType SpeedType { get; }
    public double? MagneticHeading { get; }

    private Airspeed(int speed, AirspeedType speedType, double? magneticHeading)
    {
        Speed = speed;
        SpeedType = speedType;
        MagneticHeading = magneticHeading;
    }
    
    public static VelocitySubMessage FromBits(BitArray bits, int subType)
    {
        var airspeed = bits.CopySlice(0, 10).AsInt();
        var airspeedType = bits.CopySlice(10, 1).AsInt();
        var magHeadingRaw = bits.CopySlice(11, 10).AsInt();
        var magHeadingStatus = bits.CopySlice(21, 1)[0];

        double? magHeading = null;
        if (magHeadingStatus)
        {
            magHeading = Math.Round(magHeadingRaw * 360d / 1024d, 2);
        }

        airspeed -= 1;

        if (subType == 4)
        {
            airspeed *= 4;
        }

        return new Airspeed(airspeed, (AirspeedType)airspeedType, magHeading);
    }
}