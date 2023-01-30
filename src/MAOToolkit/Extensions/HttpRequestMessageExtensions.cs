using System.Text;

namespace MAOToolkit.Extensions
{
    /// <summary>
    /// Extension methods for HTTP Request Message.
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        private const int MaxBodyLength = 8000;
        private const char SPACE = ' ';

        /// <summary>
        /// Dump the raw http request to a string.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> that should be dumped.</param>
        /// <returns>The raw HTTP request.</returns>
        public static async Task<string> ToRawAsync(this HttpRequestMessage request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var builder = new StringBuilder();

            builder
                .Append(request.Method.ToString())
                .Append(SPACE)
                .Append(request.RequestUri?.ToString())
                .Append(SPACE)
                .Append("HTTP/")
                .AppendLine(request.Version.ToString());

            foreach (var header in request.Headers)
            {
                builder.Append(header.Key).Append(':').AppendLine(String.Join(',', header.Value));
            }

            if (request.Content is not null)
            {
                foreach (var header in request.Content.Headers)
                {
                    builder.Append(header.Key).Append(':').AppendLine(String.Join(',', header.Value));
                }

                builder.AppendLine();
                builder.AppendLine(await request.Content.ReadAsStringAsync(MaxBodyLength));
            }

            return builder.ToString();
        }
    }
}