using System.Collections.Generic;
using System.Linq;

namespace Nomadify.Extensions;

public static class StringExtensions
{
    public static string AsJsonPath(this IEnumerable<string> pathParts)
    {
        var formattedParts = pathParts.Select(part => $"['{part}']");
        return "$" + string.Join("", formattedParts);
    }
}
