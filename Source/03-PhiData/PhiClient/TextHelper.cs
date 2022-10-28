using System.Text.RegularExpressions;

namespace PhiClient;

public static class TextHelper
{
    public const string SIZE = "size";

    public const string B = "b";

    public const string I = "i";

    public const string COLOR = "color";

    public static string StripRichText(string input, params string[] strippedTags)
    {
        foreach (var str in strippedTags)
        {
            input = new Regex($"<\\/?{str}(=[\\w#]+)?>").Replace(input, "");
        }

        return input;
    }

    public static string StripRichText(string input)
    {
        return StripRichText(input, "size", "b", "i", "color");
    }

    public static string Clamp(string input, int min, int max, char filler = '-')
    {
        var length = StripRichText(input).Length;
        if (length < min)
        {
            input += new string(filler, min);
        }
        else if (length > max)
        {
            input = input.Substring(0, max);
        }

        return input;
    }
}