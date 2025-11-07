using System.Text.Json.Serialization;

namespace UmaPay.Domain
{
    public class NuveiRequestTransactionData
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }


        [JsonPropertyName("bank_name")]
        public string? BankName { get; set; }

        [JsonPropertyName("bank_code")]
        public string? BankCode { get; set; }

        [JsonPropertyName("pse_cycle")]
        public string? PseCycle { get; set; }

        [JsonPropertyName("payment_method_type")]
        public string? PaymentMethodType { get; set; }

        [JsonPropertyName("order_description")]
        public string OrderDescription { get; set; }

        [JsonPropertyName("authorization_code")]
        public string AuthorizationCode { get; set; }

        [JsonPropertyName("status_detail")]
        public string StatusDetail { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("dev_reference")]
        public string DevReference { get; set; }

        [JsonPropertyName("carrier_code")]
        public string CarrierCode { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("paid_date")]
        public string PaidDate { get; set; }

        [JsonPropertyName("installments")]
        public string Installments { get; set; }

        [JsonPropertyName("ltp_id")]
        public string LtpId { get; set; }

        [JsonPropertyName("stoken")]
        public string Stoken { get; set; }

        [JsonPropertyName("application_code")]
        public string ApplicationCode { get; set; }

        [JsonPropertyName("terminal_code")]
        public string TerminalCode { get; set; }

    }
}
