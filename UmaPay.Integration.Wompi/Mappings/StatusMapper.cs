using UmaPay.Domain;
using UmaPay.Integration.Wompi.Enums;
using UmaPay.Interface.Integration.Nuvei;
using UmaPay.Resource;

namespace UmaPay.Integration.Wompi.Mappings;

public sealed class StatusMapper : IStatusMapper
{
    public TransactionStatus MapToInternalStatus(string wompiGatewayStatus)
    {
        return wompiGatewayStatus switch
        {
            nameof(WompiTransactionStatusEnum.APPROVED) => new TransactionStatus
            {
                Description = ConstStatus.GetStatusDescription(ConstStatus.Completed),
                Id = ConstStatus.Completed,
                Name = ConstStatus.GetStatusName(ConstStatus.Completed)
            },
            nameof(WompiTransactionStatusEnum.PENDING) => new TransactionStatus
            {
                Description = ConstStatus.GetStatusDescription(ConstStatus.Processing),
                Id = ConstStatus.Processing,
                Name = ConstStatus.GetStatusName(ConstStatus.Processing)
            },
            _ => new TransactionStatus
            {
                Description = ConstStatus.GetStatusDescription(ConstStatus.Failed),
                Id = ConstStatus.Failed,
                Name = ConstStatus.GetStatusName(ConstStatus.Failed)
            },
        };
    }
}
