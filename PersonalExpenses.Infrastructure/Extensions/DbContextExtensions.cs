using Microsoft.EntityFrameworkCore;

namespace PersonalExpenses.Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static DbContextOptionsBuilder ConfigureSqliteOptions(
            this DbContextOptionsBuilder optionsBuilder,
            string? connectionString)
        {
            return optionsBuilder.UseSqlite(connectionString);
        }
    }
}
