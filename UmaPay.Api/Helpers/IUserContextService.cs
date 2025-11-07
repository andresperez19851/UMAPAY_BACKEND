using System.Security.Claims;

namespace UmaPay.Api.Helpers
{
    public interface IUserContextService
    {
        int? GetApplicationCode(ClaimsPrincipal user);
    }

}
