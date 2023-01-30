using System.Text;

namespace MAOToolkit.Extensions
{
    /// <summary>
    /// Extension methods for HTTP Response Message.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        private const int MaxBodyLength = 8000;
        private const char SPACE = ' ';

        /// <summary>
        /// Dump the raw http response to a string.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/> that should be dumped.</param>
        /// <returns>The raw HTTP response.</returns>
        public static async Task<string> ToRawAsync(this HttpResponseMessage response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            var builder = new StringBuilder();

            builder
                .Append("HTTP/")
                .Append(response.Version.ToString())
                .Append(SPACE)
                .Append((int)response.StatusCode)
                .Append(SPACE)
                .AppendLine(response.ReasonPhrase);

            foreach (var header in response.Headers)
            {
                builder.Append(header.Key).Append(':').AppendLine(String.Join(',', header.Value));
            }

            if (response.Content is not null)
            {
                foreach (var header in response.Content.Headers)
                {
                    builder.Append(header.Key).Append(':').AppendLine(String.Join(',', header.Value));
                }

                builder.AppendLine();
                builder.AppendLine(await response.Content.ReadAsStringAsync(MaxBodyLength));
            }

            return builder.ToString();
        }
    }
}