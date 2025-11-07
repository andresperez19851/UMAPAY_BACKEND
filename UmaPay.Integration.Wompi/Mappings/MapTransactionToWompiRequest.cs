using UmaPay.Domain;
using UmaPay.Integration.Wompi.Contracts.PaymentLink.Request;
using UmaPay.Integration.Wompi.Settings;
using UmaPay.Resource;

namespace UmaPay.Integration.Wompi.Mappings;

public static class MapTransactionToWompiRequest
{
    public static PaymentLinkRequest ToWompiRequest(
        this Transaction transaction,
        string transactionName,
        double expirationInDays,
        string redirectUrl)
    {
        return new PaymentLinkRequest
        {
            Name = transactionName!,
            Description = transaction.Description ?? string.Empty,
            SingleUse = true,
            CollectShipping = false,
            Currency = ConstGateway.LocalCurrency,
            AmountInCents = ConvertToCents(transaction.Amount),
            ExpiresAt = DateTime.UtcNow.AddDays(expirationInDays),
            RedirectUrl = redirectUrl,
            Sku = transaction.Token.ToString(),
        };
    }

    private static long ConvertToCents(decimal amount) => (long)(amount * 100);
}
