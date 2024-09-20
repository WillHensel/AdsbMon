using System.Collections;

namespace AdsbMon.Core.Extensions;

public static class BitArrayExtensions
{
    public static int AsInt(this BitArray bits)
    {
        int[] intArray = new int[1];
        bits.CopyTo(intArray, 0);
        return intArray[0];
    }
    
    public static BitArray CopySlice(this BitArray array, int offset, int size)
    {
        var newArray = new BitArray(size);

        for (int i = 0; i < size; i++)
        {
            newArray[i] = array[offset + i];
        }

        return newArray;
    }
}