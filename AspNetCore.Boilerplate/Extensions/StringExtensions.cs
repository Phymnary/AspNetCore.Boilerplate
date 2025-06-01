using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AspNetCore.Boilerplate.Extensions;

public static partial class StringExtensions
{
    public static string PascalToKebabCase(this string value)
    {
        return PascalToKebabMyRegex().Replace(value, "-$1").Trim().ToLower();
    }

    public static string[] SplitBySemicolon(this string value)
    {
        return value
            .Split(";", StringSplitOptions.RemoveEmptyEntries)
            .Select(it => it.Trim())
            .ToArray();
    }

    public static bool IsBlank([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static string StripPostfix(this string str, string postFix)
    {
        return str.EndsWith(postFix) ? str[..^postFix.Length] : str;
    }

    public static string StripPrefix(this string str, string value)
    {
        if (str.StartsWith(value))
            str = str.Remove(0, value.Length);

        return str;
    }

    [GeneratedRegex("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z0-9])", RegexOptions.Compiled)]
    private static partial Regex PascalToKebabMyRegex();
}
