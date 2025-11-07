namespace UmaPay.Integration.Nuvei
{
    using Interface.Integration.Nuvei;
    using UmaPay.Resource;
    using Domain;

    public class StatusMapper : IStatusMapper
    {

        #region Constructor
        public StatusMapper()
        {
        }
        #endregion Constructor

        #region Public Methods

        public TransactionStatus MapToInternalStatus(string Id)
        {
            TransactionStatus statusTransaction = new TransactionStatus();

            switch (int.Parse(Id))
            {
                case Status.Paid:
                    statusTransaction = new TransactionStatus
                    {
                        Description = ConstStatus.GetStatusDescription(ConstStatus.Completed),
                        Id = ConstStatus.Completed,
                        Name = ConstStatus.GetStatusName(ConstStatus.Completed)
                    };
                    break;
                default:
                    statusTransaction = new TransactionStatus
                    {
                        Description = ConstStatus.GetStatusDescription(ConstStatus.Failed),
                        Id = ConstStatus.Failed,
                        Name = ConstStatus.GetStatusName(ConstStatus.Failed)
                    };
                    break;
            }

            return statusTransaction;
        }

        #endregion Public Methods
    }
}