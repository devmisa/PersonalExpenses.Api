using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.Security.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        (string AccessToken, string RefreshToken, int ExpiresIn) GenerateTokenPair(User user);
    }
}
