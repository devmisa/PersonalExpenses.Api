using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.Infrastructure.Interfaces
{
    public interface IExpenseRepository
    {
        Task<Expense> GetByIdAsync(int id);
        Task<(IList<Expense> Items, int TotalCount)> GetListAsync(int page, int pageSize, string? category);
        Task<Expense> AddAsync(Expense entity);
        Task<Expense> UpdateAsync(Expense entity);
        Task<Expense> DeleteAsync(Expense entity);
        Task SaveChangesAsync();
    }
}
