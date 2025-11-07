namespace UmaPay.Interface.Service
{
    using Domain;

    public interface ITransactionStatusCommandHandler
    {
        Task<OperationResult<bool>> ProcessFailedInSap();
        Task<OperationResult<bool>> ProcessFailedInSap(Guid token);
        Task<OperationResult<bool>> ProcessFailedInSap(string customer);
        Task<OperationResult<bool>> ProcessInitiated();
    }
}