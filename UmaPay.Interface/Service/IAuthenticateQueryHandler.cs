using UmaPay.Domain;

namespace UmaPay.Interface.Service
{
    public interface IAuthenticateQueryHandler
    {
        Task<OperationResult<Authorization>> AuthenticateApplicationAsync(string apiKey, string secret);
    }
}