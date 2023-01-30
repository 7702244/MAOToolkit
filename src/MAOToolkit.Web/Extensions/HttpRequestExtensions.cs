using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace MAOToolkit.Extensions
{
    /// <summary>
    /// Extension methods for HTTP Request.
    /// <remarks>
    /// See the HTTP 1.1 specification http://www.w3.org/Protocols/rfc2616/rfc2616.html
    /// for details of implementation decisions.
    /// </remarks>
    /// </summary>
    public static class HttpRequestExtensions
    {
        private const int MaxBodyLength = 8000;
        private const char SPACE = ' ';

        /// <summary>
        /// Checks if the resource has been modified since the date specified in the If-Modified-Since header.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> that should be checked.</param>
        /// <param name="updatedAt">The <see cref="DateTime"/> that should be checked.</param>
        public static bool IsModifiedSince(this HttpRequest request, DateTime updatedAt)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            // Example:
            // If-Modified-Since: <day-name>, <day> <month> <year> <hour>:<minute>:<second> GMT
            // If-Modified-Since: Wed, 21 Oct 2015 07:28:00 GMT
            var headerValue = request.Headers.IfModifiedSince.FirstOrDefault();
            if (DateTime.TryParse(headerValue, out DateTime isModifiedSince))
            {
                isModifiedSince = DateTime.SpecifyKind(isModifiedSince, DateTimeKind.Utc);
                return isModifiedSince < updatedAt.ToUniversalTime();
            }

            return true;
        }

        /// <summary>
        /// Check if request made by mobile device.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> that should be checked.</param>
        public static bool IsMobile(this HttpRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var headerValue = request.Headers.UserAgent.FirstOrDefault();
            if (!String.IsNullOrWhiteSpace(headerValue))
            {
                string u = headerValue.Trim();

                var b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                return b.IsMatch(u) || (u.Length >= 4 && v.IsMatch(u.AsSpan(0, 4)));
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> that should be checked.</param>
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            return request.Headers["X-Requested-With"].FirstOrDefault() == "XMLHttpRequest";
        }

        /// <summary>
        /// Dump the raw http request to a string.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> that should be dumped.</param>
        /// <returns>The raw HTTP request.</returns>
        public static async ValueTask<string> ToRawAsync(this HttpRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var builder = new StringBuilder();

            builder
                .Append(request.Method)
                .Append(SPACE)
                .Append(request.GetEncodedUrl())
                .Append(SPACE)
                .AppendLine(request.Protocol);

            foreach (var header in request.Headers)
            {
                builder.Append(header.Key).Append(':').AppendLine(header.Value);
            }

            builder.AppendLine();
            builder.AppendLine(await request.GetRawBodyStringAsync(MaxBodyLength));

            return builder.ToString();
        }

        /// <summary>
        /// Retrieve the raw body as a string from the Request.Body stream.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> instance to apply to.</param>
        /// <param name="charsLimit">The maximum number of chars to read.</param>
        /// <returns>String representation of Request.Body.</returns>
        public static async ValueTask<string> GetRawBodyStringAsync(this HttpRequest request, int charsLimit = 0)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            string body = String.Empty;

            if (request.ContentLength == null ||
                request.ContentLength <= 0 ||
                !request.Body.CanSeek)
            {
                return body;
            }

            request.Body.Seek(0, SeekOrigin.Begin);

            if (!request.HttpContext.RequestAborted.IsCancellationRequested)
            {
                // The `leaveOpen` should be `true` if there's another function going to be invoked AFTER this.
                using (var reader = new StreamReader(
                    request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true))
                {
                    body = await reader.ReadWithLimitAsync(charsLimit);
                }

                request.Body.Seek(0, SeekOrigin.Begin);
            }

            return body;
        }

        /// <summary>
        /// Retrieves the raw body as a byte array from the Request.Body stream.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> instance to apply to.</param>
        /// <returns>Byte array from the Request.Body stream</returns>
        public static async ValueTask<byte[]> GetRawBodyBytesAsync(this HttpRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            request.Body.Seek(0, SeekOrigin.Begin);
            using (var ms = new MemoryStream(1024))
            {
                await request.Body.CopyToAsync(ms, request.HttpContext.RequestAborted);
                request.Body.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }
    }
}