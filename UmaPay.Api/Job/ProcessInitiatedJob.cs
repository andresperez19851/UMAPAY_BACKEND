using Quartz;

namespace UmaPay.Api.Job
{
    using Interface.Service;

    public class ProcessInitiatedJob : IJob
    {
        private readonly ITransactionStatusCommandHandler _commandHandler;
        private readonly ILogger<ProcessInitiatedJob> _logger;

        public ProcessInitiatedJob(ITransactionStatusCommandHandler commandHandler, ILogger<ProcessInitiatedJob> logger)
        {
            _commandHandler = commandHandler;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
           _logger.LogInformation("Starting ProcessInitiated job");
            var result = await _commandHandler.ProcessInitiated();
            if (result.Success)
            {
                _logger.LogInformation("ProcessInitiated job completed successfully");
            }
            else
            {
                _logger.LogError($"ProcessInitiated job failed");
            }
        }
    }
}
