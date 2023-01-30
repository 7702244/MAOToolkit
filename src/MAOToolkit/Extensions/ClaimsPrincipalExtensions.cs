using System.ComponentModel;
using System.Security.Claims;

namespace MAOToolkit.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Returns the value for the first claim of the specified type, otherwise null if the claim is not present.
        /// </summary>
        /// <param name="principal">The <see cref="ClaimsPrincipal"/> instance this method extends.</param>
        /// <param name="claimType">The claim type whose first value should be returned.</param>
        /// <returns>The value of the first instance of the specified claim type, or null if the claim is not present.</returns>
        internal static string? FindFirstValue(this ClaimsPrincipal principal, string claimType)
        {
            ArgumentNullException.ThrowIfNull(principal);
            var claim = principal.FindFirst(claimType);
            return claim?.Value;
        }

        // https://stackoverflow.com/a/35577673/809357
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            ArgumentNullException.ThrowIfNull(principal);

            return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // https://ask.xiaolee.net/questions/1074898
        public static T? GetUserId<T>(this ClaimsPrincipal principal)
        {
            ArgumentNullException.ThrowIfNull(principal);

            string? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return default;

            return (T?)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(userId);
        }
    }
}