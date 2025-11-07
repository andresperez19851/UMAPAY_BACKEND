namespace UmaPay.Shared
{
    using Interface.Shared;
    using System.Threading.Tasks;

    public class Hash : IHash
    {
        public Task<bool> Verify(string text, string hashtext)
        {
            return Task.Run(() => BCrypt.Net.BCrypt.Verify(text, hashtext));
        }

        public Task<string> Generate(string text)
        {
            return Task.Run(() => BCrypt.Net.BCrypt.HashPassword(text));
        }
    }
}
