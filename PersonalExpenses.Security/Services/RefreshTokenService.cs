using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Domain.Interfaces;
using PersonalExpenses.Security.Interfaces;

namespace PersonalExpenses.Security.Services
{
    public class RefreshTokenService(IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenService
    {
        public async Task<string> StoreRefreshTokenAsync(int userId, string refreshToken, DateTime expiresAt)
        {
            RefreshToken token = new()
            {
                UserId = userId,
                Token = refreshToken,
                ExpiresAt = expiresAt,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _ = await refreshTokenRepository.AddAsync(token);
            await refreshTokenRepository.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            RefreshToken? token = await refreshTokenRepository.GetByTokenAsync(refreshToken);

            return token != null && token.UserId == userId && !token.IsRevoked && token.ExpiresAt >= DateTime.UtcNow;
        }

        public async Task<bool> RevokeRefreshTokenAsync(int userId, string refreshToken)
        {
            RefreshToken? token = await refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null || token.UserId != userId)
            {
                return false;
            }

            token.IsRevoked = true;
            await refreshTokenRepository.UpdateAsync(token);
            return true;
        }
    }
}
