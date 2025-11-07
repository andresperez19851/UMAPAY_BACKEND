using Quartz;

namespace UmaPay.Api.Job
{
    using Interface.Service;

    public class ProcessFailedInSapJob : IJob
    {
        #region Properties

        private readonly ITransactionStatusCommandHandler _commandHandler;
        private readonly ILogger<ProcessFailedInSapJob> _logger;

        #endregion Properties

        #region Constructor

        public ProcessFailedInSapJob(ITransactionStatusCommandHandler commandHandler, ILogger<ProcessFailedInSapJob> logger)
        {
            _commandHandler = commandHandler;
            _logger = logger;
        }

        #endregion Constructor

        #region Task

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Starting ProcessFailedInSap job");
            var result = await _commandHandler.ProcessFailedInSap();
            if (result.Success)
            {
                _logger.LogInformation("ProcessFailedInSap job completed successfully");
            }
            else
            {
                _logger.LogError($"ProcessFailedInSap job failed");
            }
        }

        #endregion Task
    }
}
