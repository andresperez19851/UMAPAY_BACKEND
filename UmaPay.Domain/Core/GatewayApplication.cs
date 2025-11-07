namespace UmaPay.Domain
{
    public class GatewayApplication
    {
        public int GatewayId { get; set; }
        public int ApplicationId { get; set; }
        public Gateway Gateway { get; set; }
        public Application Application { get; set; }
    }
}