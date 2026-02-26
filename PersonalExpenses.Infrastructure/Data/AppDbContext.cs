using Microsoft.EntityFrameworkCore;
using PersonalExpenses.Domain.Entities;
using PersonalExpenses.Infrastructure.Data.Configurations;

namespace PersonalExpenses.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _ = modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            _ = modelBuilder.ApplyConfiguration(new UserConfiguration());
            _ = modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        }
    }
}
