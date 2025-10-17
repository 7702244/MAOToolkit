using System.Net;
using Microsoft.AspNetCore.Http;

namespace MAOToolkit.Extensions;

/// <summary>
/// Extension methods for HTTP Context.
/// <remarks>
/// See the HTTP 1.1 specification http://www.w3.org/Protocols/rfc2616/rfc2616.html
/// for details of implementation decisions.
/// </remarks>
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Obtaining IP address from HttpContext.
    /// </summary>
    /// <param name="context">Http context</param>
    /// <returns>IPAddress</returns>
    public static IPAddress? GetClientIpAddress(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        IPAddress? ipAddress;

        // Handle standardized 'Forwarded' header.
        // Examples:
        // Forwarded: for="_gazonk"
        // Forwarded: For="[2001:db8:cafe::17]:4711"
        // Forwarded: for=192.0.2.60;proto=http;by=203.0.113.43
        // Forwarded: for=192.0.2.43, for=198.51.100.17
        string forwarded = context.Request.Headers["Forwarded"].ToString();
        if (!String.IsNullOrEmpty(forwarded))
        {
            foreach (string segment in forwarded.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                string[] pair = segment.Trim().Split('=');
                if (pair.Length == 2 && pair[0].Equals("for", StringComparison.OrdinalIgnoreCase))
                {
                    // Trim quotes.
                    string ip = pair[1].Trim('"');

                    if (String.IsNullOrWhiteSpace(ip))
                        continue;

                    // IPv6 addresses are always enclosed in square brackets.
                    int left = ip.IndexOf('['), right = ip.IndexOf(']');
                    if (left == 0 && right > 0)
                    {
                        ip = ip.Substring(1, right - 1);
                    }

                    // if only IPv6 with/without brackets.
                    if (IPAddress.TryParse(ip, out ipAddress))
                        return ipAddress;

                    // Strip port of IPv4 addresses.
                    int colon = ip.IndexOf(':');
                    if (colon != -1)
                    {
                        ip = ip.Substring(0, colon);
                    }

                    // This will return IPv4.
                    if (IPAddress.TryParse(ip, out ipAddress) && !ipAddress.IsPrivateNetwork())
                        return ipAddress;
                }
            }
        }

        // Handle non-standardized 'CF-Connecting-IP' header.
        string cfConnectingIp = context.Request.Headers["CF-Connecting-IP"].ToString();
        if (!String.IsNullOrEmpty(cfConnectingIp))
        {
            foreach (string ip in cfConnectingIp.Split(','))
            {
                if (String.IsNullOrWhiteSpace(ip))
                    continue;

                if (IPAddress.TryParse(ip.Trim(), out ipAddress) && !ipAddress.IsPrivateNetwork())
                    return ipAddress;
            }
        }

        // Handle non-standardized 'X-Forwarded-For' header.
        string xForwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
        if (!String.IsNullOrEmpty(xForwardedFor))
        {
            foreach (string ip in xForwardedFor.Split(','))
            {
                if (String.IsNullOrWhiteSpace(ip))
                    continue;

                if (IPAddress.TryParse(ip.Trim(), out ipAddress) && !ipAddress.IsPrivateNetwork())
                    return ipAddress;
            }
        }

        // Handle non-standardized 'Client-IP' header.
        string clientIp = context.Request.Headers["Client-IP"].ToString();
        if (!String.IsNullOrEmpty(clientIp))
        {
            foreach (string ip in clientIp.Split(','))
            {
                if (String.IsNullOrWhiteSpace(ip))
                    continue;

                if (IPAddress.TryParse(ip.Trim(), out ipAddress) && !ipAddress.IsPrivateNetwork())
                    return ipAddress;
            }
        }

        // Handle real IP in 'REMOTE_ADDR' header.
        return context.Connection.RemoteIpAddress;
    }
}