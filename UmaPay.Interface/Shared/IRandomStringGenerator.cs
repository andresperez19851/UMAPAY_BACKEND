namespace UmaPay.Interface.Shared
{
    public interface IRandomStringGenerator
    {
        Task<string> GenerateAsync(int length);
    }
}