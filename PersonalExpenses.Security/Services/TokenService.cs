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
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(settings.Secret);

            SecurityTokenDescriptor tokenDescriptor = new()
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

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[64];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public (string AccessToken, string RefreshToken, int ExpiresIn) GenerateTokenPair(User user)
        {
            string accessToken = GenerateToken(user);
            string refreshToken = GenerateRefreshToken();
            int expiresIn = settings.ExpiryInHours * 3600;

            return (accessToken, refreshToken, expiresIn);
        }
    }
}

