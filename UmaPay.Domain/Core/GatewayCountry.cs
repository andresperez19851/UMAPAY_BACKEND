namespace UmaPay.Domain
{
    public class GatewayCountry
    {
        public int GatewayId { get; set; }
        public int CountryId { get; set; }
        public Gateway Gateway { get; set; }
        public Country Country { get; set; }
    }
}