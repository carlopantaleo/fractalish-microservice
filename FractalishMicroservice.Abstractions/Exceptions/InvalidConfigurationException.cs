namespace FractalishMicroservice.Abstractions.Exceptions;

public class InvalidConfigurationException(string message) : Exception(message)
{
    public static void ThrowIfNullOrWhitespace(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidConfigurationException(message);
        }
    }
}
