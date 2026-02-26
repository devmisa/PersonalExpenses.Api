using Microsoft.EntityFrameworkCore;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Domain.Interfaces;
using PersonalExpenses.Infrastructure.Data;

namespace PersonalExpenses.Infrastructure.Repositories
{
    public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
    {
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<IReadOnlyList<RefreshToken>> GetByUserIdAsync(int userId)
        {
            return await context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();
        }

        public async Task<RefreshToken> AddAsync(RefreshToken token)
        {
            _ = await context.RefreshTokens.AddAsync(token);
            return token;
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            _ = context.RefreshTokens.Update(token);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            _ = await context.SaveChangesAsync();
        }
    }
}
