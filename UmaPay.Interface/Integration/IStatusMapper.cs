namespace UmaPay.Interface.Integration.Nuvei
{
    using Domain;
    public interface IStatusMapper
    {
        TransactionStatus MapToInternalStatus(string Id);
    }
}
