using System.Text;

namespace MAOToolkit.Extensions
{
    /// <summary>
    /// Extension methods for HTTP content.
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Serialize the HTTP content to a string with specified <paramref name="charsLimit" /> as an asynchronous operation.
        /// </summary>
        /// <param name="httpContent">The <see cref="HttpContent"/> instance to apply to.</param>
        /// <param name="charsLimit">The maximum number of chars to read.</param>
        /// <returns>String representation of HTTP content.</returns>
        public static async Task<string> ReadAsStringAsync(this HttpContent httpContent, int charsLimit)
        {
            ArgumentNullException.ThrowIfNull(httpContent);
            
            var stream = await httpContent.ReadAsStreamAsync();
            if (!stream.CanRead)
            {
                return "<UNREADABLE_BODY_STREAM>";
            }

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            // The `leaveOpen` should be `true` if there's another function going to be invoked AFTER this.
            using var reader = new StreamReader(
                stream,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);
            string result = await reader.ReadWithLimitAsync(charsLimit);

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            
            return result;
        }
    }
}