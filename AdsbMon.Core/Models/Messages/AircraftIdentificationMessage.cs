using System.Collections;
using AdsbMon.Core.Extensions;

namespace AdsbMon.Core.Models.Messages;

public class AircraftIdentificationMessage : SubMessage
{
    public AircraftCategory Category { get; set; }
    public string Callsign { get; set; }

    private static Dictionary<int, AircraftCategory> _typeCode2Map = new()
    {
        { 1, AircraftCategory.SurfaceEmergencyVehicle },
        { 3, AircraftCategory.SurfaceServiceVehicle },
        { 4, AircraftCategory.GroundObstruction },
        { 5, AircraftCategory.GroundObstruction },
        { 6, AircraftCategory.GroundObstruction },
        { 7, AircraftCategory.GroundObstruction }
    };

    private static Dictionary<int, AircraftCategory> _typeCode3Map = new()
    {
        { 1, AircraftCategory.GliderOrSailplane },
        { 2, AircraftCategory.LighterThanAir },
        { 3, AircraftCategory.ParachutistOrSkydiver },
        { 4, AircraftCategory.UltralightOrHangGliderOrParaglider },
        { 5, AircraftCategory.Reserved },
        { 6, AircraftCategory.UnmannedAerialVehicle },
        { 7, AircraftCategory.SpaceOrTransatmosphericVehicle }
    };

    private static Dictionary<int, AircraftCategory> _typeCode4Map = new()
    {
        { 1, AircraftCategory.Light },
        { 2, AircraftCategory.Medium1 },
        { 3, AircraftCategory.Medium2 },
        { 4, AircraftCategory.HighVortex },
        { 5, AircraftCategory.Heavy },
        { 6, AircraftCategory.HighPerformance },
        { 7, AircraftCategory.Rotorcraft }
    };

    private static Dictionary<int, Dictionary<int, AircraftCategory>> _typeCodeMap = new()
    {
        { 2, _typeCode2Map },
        { 3, _typeCode3Map },
        { 4, _typeCode4Map }
    };

    private AircraftIdentificationMessage(AircraftCategory category, string callsign)
    {
        Category = category;
        Callsign = callsign;
    }

    public static SubMessage FromHex(string hex)
    {
        var tcca = hex.Substring(0, 2);
        var rest = hex.Substring(2);

        var tccaBits = new BitArray(Convert.FromHexString(tcca));
        var bytes = Convert.FromHexString(rest);
        Array.Reverse(bytes);
        var bits = new BitArray(bytes);

        var categoryNum = tccaBits.CopySlice(0, 3).AsInt();
        var typeCode = tccaBits.CopySlice(3, 5).AsInt();

        AircraftCategory category;
        if (typeCode == 1 || categoryNum == 0)
        {
            category = AircraftCategory.None;
        }
        else
        {
            try
            {
                var categoryMap = _typeCodeMap[typeCode];
                category = categoryMap[categoryNum];
            }
            catch 
            {
                category = AircraftCategory.None;
            }
        }

        var callsign = GetCallsignFromBits(bits);

        return new AircraftIdentificationMessage(category, callsign);
    }

    private static string GetCallsignFromBits(BitArray bits)
    {
        string result = "";

        for (int i = 0; i < 8; i++)
        {
            int charNum = bits.CopySlice(6 * i, 6).AsInt();
            
            if (charNum is >= 1 and <= 26)
            {
                charNum += 64;
            }
            
            result = (char)charNum + result;
        }

        return result;
    }
}