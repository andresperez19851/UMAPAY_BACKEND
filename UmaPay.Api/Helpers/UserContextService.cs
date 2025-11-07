using System.Security.Claims;

namespace UmaPay.Api.Helpers
{
    public class UserContextService : IUserContextService
    {
        public int? GetApplicationCode(ClaimsPrincipal user)
        {
            var applicationClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(applicationClaim, out var applicationCode) ? applicationCode : (int?)null;
        }
    }
}
