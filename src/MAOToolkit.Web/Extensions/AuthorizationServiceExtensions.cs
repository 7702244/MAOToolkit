using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace MAOToolkit.Extensions
{
    public static class AuthorizationServiceExtensions
    {
        public static Task<AuthorizationResult> AuthorizeAsync(this IAuthorizationService service, ClaimsPrincipal user, IAuthorizationRequirement requirement)
        {
            return service.AuthorizeAsync(user, null, requirement);
        }
    }
}