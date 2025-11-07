namespace UmaPay.Domain
{
    public record GatewayPaymentRequest
    {
        public required string Code { get; set; }
        public required string Currency { get; set; }
        public required string email { get; set; }
    }

}
