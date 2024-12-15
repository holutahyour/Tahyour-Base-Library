namespace Tahyour.Base.Common.Domain.Utilities;

public static class StringExtensions
{
    /// <summary>
    /// Converts a string to CamelCase.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The CamelCase version of the string.</returns>
    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        input = input.Trim();
        return char.ToLowerInvariant(input[0]) + input.Substring(1);
    }

    /// <summary>
    /// Converts a string to PascalCase.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The PascalCase version of the string.</returns>
    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        input = input.Trim();
        return char.ToUpperInvariant(input[0]) + input.Substring(1);
    }

    /// <summary>
    /// Capitalizes the first letter of each word in the string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The capitalized version of the string.</returns>
    public static string ToCapitalized(this string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;

        input = input.Trim();
        var words = input.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(words[i]))
            {
                words[i] = char.ToUpperInvariant(words[i][0]) + words[i].Substring(1).ToLowerInvariant();
            }
        }
        return string.Join(" ", words);
    }
}
