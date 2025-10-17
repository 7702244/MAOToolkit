namespace MAOToolkit.Extensions;

public static class StreamReaderExtensions
{
    /// <summary>
    /// Reads characters with specified <paramref name="charsLimit"/> from the current
    /// position to the end of the stream asynchronously and returns them as one string.
    /// </summary>
    /// <param name="reader">The <see cref="StreamReader"/> instance to apply to.</param>
    /// <param name="charsLimit">The maximum number of chars to read.</param>
    /// <returns>String representation of readed stream.</returns>
    public static async ValueTask<string> ReadWithLimitAsync(this StreamReader reader, int charsLimit)
    {
        ArgumentNullException.ThrowIfNull(reader);

        if (charsLimit > 0)
        {
            char[] chars = new char[charsLimit];
            int read = await reader.ReadAsync(chars, 0, chars.Length);
            return new string(chars.AsSpan(0, read));
        }

        return await reader.ReadToEndAsync();
    }
}