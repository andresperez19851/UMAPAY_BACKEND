namespace UmaPay.Interface.Shared
{
    public interface IHash
    {
        Task<string> Generate(string text);
        Task<bool> Verify(string text, string hashtext);
    }
}
