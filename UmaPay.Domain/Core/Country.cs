namespace UmaPay.Domain
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public ICollection<GatewayCountry> GatewayCountries { get; set; }
    }
}