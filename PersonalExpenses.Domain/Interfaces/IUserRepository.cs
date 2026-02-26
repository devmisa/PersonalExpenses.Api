using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<User> AddAsync(User entity);
        Task SaveChangesAsync();
    }
}
