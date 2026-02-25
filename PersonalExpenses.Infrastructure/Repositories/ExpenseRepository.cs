using Microsoft.EntityFrameworkCore;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Infrastructure.Data;
using PersonalExpenses.Infrastructure.Interfaces;

namespace PersonalExpenses.Infrastructure.Repositories
{
    public class ExpenseRepository(AppDbContext context) : IExpenseRepository
    {
        public async Task<Expense> GetByIdAsync(int id)
        {
            return await GetExpenseByIdOrThrowAsync(id);
        }

        public async Task<(IList<Expense> Items, int TotalCount)> GetListAsync(int page, int pageSize, string? category)
        {
            var query = context.Set<Expense>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(e => e.Category == category);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(e => e.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Expense> AddAsync(Expense entity)
        {
            _ = await context.Set<Expense>().AddAsync(entity);
            return entity;
        }

        public async Task<Expense> UpdateAsync(Expense entity)
        {
            Expense existingEntity = await GetExpenseByIdOrThrowAsync(entity.Id);

            context.Entry(existingEntity).CurrentValues.SetValues(entity);
            return existingEntity;
        }

        public async Task<Expense> DeleteAsync(Expense entity)
        {
            Expense existingEntity = await GetExpenseByIdOrThrowAsync(entity.Id);

            _ = context.Set<Expense>().Remove(existingEntity);
            return existingEntity;
        }

        public async Task SaveChangesAsync()
        {
            _ = await context.SaveChangesAsync();
        }


        #region private methods
        private async Task<Expense> GetExpenseByIdOrThrowAsync(int id)
        {
            return await context.Set<Expense>().FindAsync(id)
                ?? throw new KeyNotFoundException($"Expense with id {id} not found.");
        }
        #endregion
    }
}
