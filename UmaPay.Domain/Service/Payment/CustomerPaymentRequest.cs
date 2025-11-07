namespace UmaPay.Domain
{
    public record CustomerPaymentRequest
    {
        public required string Society { get; set; }
        public required string CodeSap { get; set; }
        public required string LastName { get; set; }
        public required string FirstName { get; set; }
        public required string Email { get; set; }
        public required string User { get; set; }

    }
}
