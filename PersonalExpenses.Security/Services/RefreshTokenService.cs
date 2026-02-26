using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Domain.Interfaces;
using PersonalExpenses.Security.Interfaces;

namespace PersonalExpenses.Security.Services
{
    public class RefreshTokenService(IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenService
    {
        public async Task<string> StoreRefreshTokenAsync(int userId, string refreshToken, DateTime expiresAt)
        {
            var token = new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiresAt = expiresAt,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await refreshTokenRepository.AddAsync(token);
            await refreshTokenRepository.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            var token = await refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null)
                return false;

            if (token.UserId != userId)
                return false;

            if (token.IsRevoked)
                return false;

            if (token.ExpiresAt < DateTime.UtcNow)
                return false;

            return true;
        }

        public async Task<bool> RevokeRefreshTokenAsync(int userId, string refreshToken)
        {
            var token = await refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (token == null || token.UserId != userId)
                return false;

            token.IsRevoked = true;
            await refreshTokenRepository.UpdateAsync(token);
            return true;
        }
    }
}
