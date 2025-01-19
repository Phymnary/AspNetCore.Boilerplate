using System.Text.RegularExpressions;

namespace AspNetCore.Boilerplate.Extensions;

public static partial class StringExtensions
{
    [GeneratedRegex("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])", RegexOptions.Compiled)]
    private static partial Regex PascalToKebabMyRegex();

    public static string PascalToKebabCase(this string value)
    {
        return PascalToKebabMyRegex().Replace(value, "-$1").Trim().ToLower();
    }

    public static string RemovePostFix(this string url, string postFix)
    {
        return url.EndsWith(postFix) ? url[..^postFix.Length] : url;
    }
}
