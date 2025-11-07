namespace UmaPay.Interface.Service
{
    using UmaPay.Domain;

    public interface IApplicationCommandHandler
    {
        Task<OperationResult<Application>> CreateAsync(string applicationName);
        Task<OperationResult<string>> RegenerateSecretAsync(string apiKey);
    }
}