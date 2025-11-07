using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UmaPay.Service
{
    using Interface.Service;
    using Resource;
    using Domain;

    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        #region Properties

        private readonly IConfiguration _configuration;

        #endregion Properties

        #region Constructor

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion Constructor

        #region Public Methods

        public string GenerateToken(Application application, Guid transaction)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["ApiSettings:Secret"] ?? throw new InvalidOperationException(Message.JWTSecretRequired));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, application.Name),
                new Claim(ClaimTypes.NameIdentifier, application.Id.ToString()),
                new Claim(ConstClaim.Transaction, transaction.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            if (!int.TryParse(_configuration["ApiSettings:TokenExpirationMinutes"], out int expirationMinutes))
            {
                expirationMinutes = 60;
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Issuer = _configuration["ApiSettings:Issuer"],
                Audience = _configuration["ApiSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion Public Methods
    }
}