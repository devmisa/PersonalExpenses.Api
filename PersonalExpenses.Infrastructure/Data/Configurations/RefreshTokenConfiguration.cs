using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalExpenses.Domain.Entities;

namespace PersonalExpenses.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            _ = builder.ToTable("RefreshTokens");

            _ = builder.HasKey(rt => rt.Id);
            _ = builder.Property(rt => rt.Id)
                .ValueGeneratedOnAdd();

            _ = builder.Property(rt => rt.UserId)
                .IsRequired();

            _ = builder.Property(rt => rt.Token)
                .IsRequired();

            _ = builder.Property(rt => rt.ExpiresAt)
                .IsRequired();

            _ = builder.Property(rt => rt.IsRevoked)
                .IsRequired();

            _ = builder.Property(rt => rt.CreatedAt)
                .IsRequired();

            _ = builder.HasIndex(rt => rt.Token)
                .IsUnique();

            _ = builder.HasIndex(rt => rt.UserId);

            _ = builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
