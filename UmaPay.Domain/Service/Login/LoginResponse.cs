namespace UmaPay.Domain
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Url { get; set; }
        public Guid Transaction { get; set; }
    }
}
