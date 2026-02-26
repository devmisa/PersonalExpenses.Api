using Microsoft.EntityFrameworkCore;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Domain.Interfaces;
using PersonalExpenses.Infrastructure.Data;

namespace PersonalExpenses.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await context.Set<User>().FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> AddAsync(User entity)
        {
            await context.Set<User>().AddAsync(entity);
            return entity;
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}

