namespace FractalishMicroservice.Abstractions.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an invalid configuration is encountered.
/// </summary>
public class InvalidConfigurationException(string message) : Exception(message)
{
    /// <summary>
    /// Throws an <see cref="InvalidConfigurationException"/> if the specified value is null, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="message">The message of the exception.</param>
    /// <exception cref="InvalidConfigurationException">Thrown if <paramref name="value"/> is null, empty, or consists only of white-space characters.</exception>
    public static void ThrowIfNullOrWhitespace(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidConfigurationException(message);
        }
    }
}
