namespace UmaPay.Domain
{
    public class CountryPaymentRequest
    {
        public string Name { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
    }
}