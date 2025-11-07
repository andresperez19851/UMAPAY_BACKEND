namespace UmaPay.Middleware.Sap.Service;

public sealed class SAPSettings
{
    public string? PaymentEndpoint { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public bool?ProcessFailedInSap { get; set; }
}
