using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            _ = builder.ToTable("Users");

            _ = builder.HasKey(u => u.Id);
            _ = builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            _ = builder.Property(u => u.Name)
                .IsRequired();

            _ = builder.Property(u => u.Email)
                .IsRequired();

            _ = builder.Property(u => u.PasswordHash)
                .IsRequired();
        }
    }
}
