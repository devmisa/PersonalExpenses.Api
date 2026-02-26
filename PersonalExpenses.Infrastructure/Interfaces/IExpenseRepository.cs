using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.Infrastructure.Interfaces
{
    public interface IExpenseRepository
    {
        Task<Expense> GetByIdAsync(int id, int userId);
        Task<(IReadOnlyList<Expense> Items, int TotalCount)> GetListAsync(int page, int pageSize, string? category, int userId);
        Task<Expense> AddAsync(Expense entity);
        Task<Expense> UpdateAsync(Expense entity);
        Task<Expense> DeleteAsync(Expense entity);
        Task SaveChangesAsync();
    }
}
