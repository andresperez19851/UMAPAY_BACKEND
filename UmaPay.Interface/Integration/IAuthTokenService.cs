namespace UmaPay.Interface.Integration.Nuvei
{
    public interface IAuthTokenService
    {
        string GenerateToken();
        bool VerifyStoken(string stoken, string transaction, string user);
    }
}