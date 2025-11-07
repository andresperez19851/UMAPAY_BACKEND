using System.Security.Cryptography;

namespace UmaPay.Shared
{
    using Interface.Shared;
    using static System.Net.Mime.MediaTypeNames;
    using System.Collections.Generic;

    public class RandomStringGenerator : IRandomStringGenerator
    {
        public async Task<string> GenerateAsync(int length)
        {
            return await Task.Run(() =>
            {
                using (var rng = new RNGCryptoServiceProvider())
                {
                    var bits = (length * 6);
                    var byte_size = ((bits + 7) / 8);
                    var bytesarray = new byte[byte_size];
                    rng.GetBytes(bytesarray);
                    return Convert.ToBase64String(bytesarray);
                }
            });
        }
    }
}
