namespace Tahyour.Base.Common.Domain.Utilities;

public static class RandomGenerator
{
    public static int RandomNumber(int min, int max)
    {
        Random _random = new Random();

        return _random.Next(min, max);
    }

    public static string RandomString(int size, bool lowerCase = false)
    {
        Random _random = new Random();

        var builder = new StringBuilder(size);

        char offset = lowerCase ? 'a' : 'A';
        const int lettersOffset = 26;

        for (var i = 0; i < size; i++)
        {
            var @char = (char)_random.Next(offset, offset + lettersOffset);
            builder.Append(@char);
        }

        return lowerCase ? builder.ToString().ToLower() : builder.ToString();
    }

    public static string RandomPassword()
    {
        Random _random = new Random();

        var passwordBuilder = new StringBuilder();

        passwordBuilder.Append(RandomString(4, true));

        passwordBuilder.Append(RandomNumber(1000, 9999));

        passwordBuilder.Append(RandomString(2));

        return passwordBuilder.ToString();
    }


}
