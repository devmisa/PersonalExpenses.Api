namespace PersonalExpenses.Security.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> StoreRefreshTokenAsync(int userId, string refreshToken, DateTime expiresAt);
        Task<bool> ValidateRefreshTokenAsync(int userId, string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(int userId, string refreshToken);
    }
}
