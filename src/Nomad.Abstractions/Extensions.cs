using System.Collections.Generic;
using System.Text.Json;

namespace Nomad.Abstractions;

public static class Extensions
{
    public static string ToValidHcl(this string s)
    {
        s = s
            .Replace("\uFEFF", "");

        return s;
    }

    public static bool IsPrimitive(this string s) => int.TryParse(s, out _) || bool.TryParse(s, out _);

    public static string DoubleQuoted(this string s) => $"\"{s}\"";

    public static IEnumerable<(string Key, object Value)> WithCorrectedBooleanValues(this IEnumerable<(string Key, object Value)> keyValuePairs)
    {
        var result = new List<(string, object)>();

        foreach (var keyValuePair in keyValuePairs)
        {
            if (keyValuePair.Value is not JsonElement jsonElement)
            {
                continue;
            }

            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.True:
                    result.Add(((keyValuePair.Key, bool.TrueString.ToLowerInvariant())));
                    break;
                case JsonValueKind.False:
                    result.Add((keyValuePair.Key, bool.FalseString.ToLowerInvariant()));
                    break;
                default:
                    result.Add(keyValuePair);
                    break;
            }
        }

        return result;
    }
}