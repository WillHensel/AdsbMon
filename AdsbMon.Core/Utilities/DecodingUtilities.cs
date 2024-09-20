using System.Collections;

namespace AdsbMon.Core.Utilities;

public static class DecodingUtilities
{
    public static double FlooredMod(double x, double y)
    {
        return x - y * Math.Floor(x / y);
    }
}