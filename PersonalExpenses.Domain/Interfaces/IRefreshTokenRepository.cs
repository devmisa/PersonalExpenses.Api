using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<List<RefreshToken>> GetByUserIdAsync(int userId);
        Task<RefreshToken> AddAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token);
        Task SaveChangesAsync();
    }
}
