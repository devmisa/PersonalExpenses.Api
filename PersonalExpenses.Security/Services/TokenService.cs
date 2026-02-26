using Microsoft.IdentityModel.Tokens;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Security.Interfaces;
using PersonalExpenses.Security.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PersonalExpenses.Security.Services
{
    public class TokenService(JwtSettings settings) : ITokenService
    {
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(settings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            ]),
                Expires = DateTime.UtcNow.AddHours(settings.ExpiryInHours),
                Issuer = settings.Issuer,
                Audience = settings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public (string AccessToken, string RefreshToken, int ExpiresIn) GenerateTokenPair(User user)
        {
            var accessToken = GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            var expiresIn = settings.ExpiryInHours * 3600; 

            return (accessToken, refreshToken, expiresIn);
        }
    }
}

