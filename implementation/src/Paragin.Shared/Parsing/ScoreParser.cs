using System.Globalization;

namespace Paragin.Shared.Parsing;

internal static class ScoreParser
{
    public static bool TryParseScore(string? raw, out double value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        return double.TryParse(raw.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    public static double ScoreForTotal(string? raw)
    {
        return TryParseScore(raw, out var value) ? value : 0;
    }
}
