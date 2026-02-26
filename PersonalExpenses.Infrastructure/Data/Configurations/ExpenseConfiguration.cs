using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.Infrastructure.Data.Configurations
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            _ = builder.ToTable("Expenses");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            _ = builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);

            _ = builder.Property(e => e.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            _ = builder.Property(e => e.Date)
                .IsRequired();

            _ = builder.Property(e => e.Category)
                .HasMaxLength(100);

            _ = builder.Property(e => e.UserId)
                .IsRequired();

            _ = builder.HasIndex(e => e.UserId);

            _ = builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
