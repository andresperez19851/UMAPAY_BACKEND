using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace UmaPay.Integration.Nuvei
{

    using Interface.Integration.Nuvei;
    using UmaPay.Domain;

    public class AuthTokenService : IAuthTokenService
    {

        #region Properties
        
        private readonly string _applicationCode;
        private readonly string _applicationKey;

        #endregion Properties

        #region Constructor

        public AuthTokenService(IConfiguration configuration)
        {
            _applicationCode = configuration["Nuvei:ApplicationCode"]!;
            _applicationKey = configuration["Nuvei:ApplicationKey"]!;
        }

        #endregion Constructor

        #region Public Methods

        public string GenerateToken()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var keyTime = _applicationKey + timestamp;
            var uniqToken = ComputeSha256Hash(keyTime);
            var strUnion = $"{_applicationCode};{timestamp};{uniqToken}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(strUnion));
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


        public bool VerifyStoken(string stoken, string transaction, string user)
        {
            var data = $"{transaction}_{_applicationCode}_{user}_{_applicationKey}";
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(data);
            var hashBytes = md5.ComputeHash(inputBytes);

            var generatedStoken = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            return generatedStoken == stoken;
        }

        #endregion Private Methods

    }

}