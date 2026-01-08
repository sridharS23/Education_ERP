using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Data.Configurations;

public class ParentConfiguration : IEntityTypeConfiguration<Parent>
{
    public void Configure(EntityTypeBuilder<Parent> builder)
    {
        builder.ToTable("parents");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Relationship)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Occupation)
            .HasMaxLength(100);

        builder.Property(p => p.AnnualIncome)
            .HasPrecision(15, 2);

        // Indexes
        builder.HasIndex(p => p.UserId)
            .IsUnique()
            .HasDatabaseName("idx_parents_user_id");
    }
}
