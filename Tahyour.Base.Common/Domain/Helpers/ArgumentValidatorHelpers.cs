internal static class ArgumentValidatorHelpers
{
    public static void ValidateArgument<TArg>(TArg argument, string argumentName)
    {
        if (argument == null || (argument is string str && string.IsNullOrWhiteSpace(str)))
        {
            throw new ArgumentNullException(argumentName, $"{argumentName} cannot be null or empty.");
        }
    }

    public static void ValidateStringArgument(string argument, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentException($"{argumentName} cannot be null or empty.", argumentName);
        }
    }
}