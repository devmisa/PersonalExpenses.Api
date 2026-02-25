using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PersonalExpenses.Infrastructure.Data;
using PersonalExpenses.Infrastructure.Extensions;

namespace PersonalExpenses.Infrastructure.DesignTime
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "../PersonalExpenses.Api");
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            DbContextOptionsBuilder<AppDbContext> optionsBuilder = new();
            optionsBuilder.ConfigureSqliteOptions(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
