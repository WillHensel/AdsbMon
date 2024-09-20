namespace AdsbMon.Core.Models.Messages;

public interface SubMessage
{
    public static abstract SubMessage FromHex(string hex);
}