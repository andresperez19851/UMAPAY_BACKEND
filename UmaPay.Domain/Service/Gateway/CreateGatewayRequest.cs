namespace UmaPay.Domain
{
    public class CreateGatewayRequest
    {
        public required string Gateway { get; set; }
        public required string Application { get; set; }
        public required string Country { get; set; }

    }
}
