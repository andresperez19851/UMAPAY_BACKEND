namespace UmaPay.Domain
{
    public record Customer
    {
        public int Id { get; set; }
        public required string Society { get; set; }
        public required string CodeSap { get; set; }
        public required string LastName { get; set; }
        public required string FirstName { get; set; }
        public required string Email { get; set; }
        public string? User { get; set; }
    }

}
