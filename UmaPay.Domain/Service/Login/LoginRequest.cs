
using System.ComponentModel.DataAnnotations;

namespace UmaPay.Domain
{

    using Resource;

    public class LoginRequest
    {
        [Required(ErrorMessage = Required.Application)]
        public string ApiKey { get; set; }

        [Required(ErrorMessage = Required.Secret)]
        public string Secret { get; set; }

        public string? Gateway { get; set; }

        [Required(ErrorMessage = Required.Invoces)]
        public required CustomerPaymentRequest Customer { get; init; }

        [Required(ErrorMessage = Required.Customer)]
        public required List<InvoicePaymentRequest> Invoices { get; init; } = new();

        [Required(ErrorMessage = Required.Country)]
        public string Country { get; set; }

    }
}
