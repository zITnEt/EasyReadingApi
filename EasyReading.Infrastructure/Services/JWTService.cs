using EasyReading.Application.Abstractions;
using EasyReading.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace EasyReading.Infrastructure.Services
{
    public class JWTService : ITokenService
    {
        private readonly JWTConfiguration _jwtConfiguration;
        public JWTService(IOptions<JWTConfiguration> jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration.Value;
        }

        public string GetAccessToken(Claim[] userClaims)
        {
            var jwtClaims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
            };

            var claims = userClaims.Concat(jwtClaims);

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SecretJWT") ?? "")),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                _jwtConfiguration.ValidIssuer,
                _jwtConfiguration.ValidAudience,
                claims,
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
