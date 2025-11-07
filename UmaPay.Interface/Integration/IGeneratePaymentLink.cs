namespace UmaPay.Interface.Integration.Nuvei;

using Domain;

public interface IGeneratePaymentLink
{
    Task<OperationResult<Transaction>> GeneratePaymentLinkAsync(Transaction domain);
}