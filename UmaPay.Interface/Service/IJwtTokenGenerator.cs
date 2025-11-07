namespace UmaPay.Interface.Service
{
    using Domain;

    public interface IJwtTokenGenerator
    {
        string GenerateToken(Application application, Guid transaction);
    }
}
