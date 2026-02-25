using Microsoft.EntityFrameworkCore;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Infrastructure.Data.Configurations;

namespace PersonalExpenses.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _ = modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
        }
    }
}
