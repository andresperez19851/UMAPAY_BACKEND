using System.Text.Json.Serialization;

namespace UmaPay.Domain
{
    public class TransactionReport
    {
        // Información de la transacción
        public int Id { get; set; }

        //Factura
        public string Billing { get; set; }


        //Valor pago
        public decimal AmountGateway { get; set; }

        //Pasarela
        public string Gateway { get; set; }

        //Pasarela
        public string Reference { get; set; }

        //Valor pago
        public decimal AmountBilling { get; set; }

        //Token
        public Guid Token { get; set; }

        //Pago Pasarela
        public DateTime TransactionDate { get; set; }

        //Pago SAP
        public DateTime? SapDate { get; set; }

        [JsonIgnore]
        public string StatusName { get; set; }

        public string Status { get; set; }

        [JsonIgnore]
        public string CodeSap { get; set; }

        public string DocumentSap { get; set; }

    }
}

