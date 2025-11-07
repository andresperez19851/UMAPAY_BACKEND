namespace UmaPay.Domain
{
    public class AuthTokenResponserew
    {
        public long Timestamp { get; set; }
        public string UniqTokenString { get; set; }
        public string UniqTokenHash { get; set; }
        public string AuthToken { get; set; }
        public override string ToString()
        {
            return $"TIMESTAMP: {Timestamp}\n" +
                   $"UNIQTOKENST: {UniqTokenString}\n" +
                   $"UNIQTOHAS: {UniqTokenHash}\n" +
                   $"AUTHTOKEN: {AuthToken}";
        }
    }
}
