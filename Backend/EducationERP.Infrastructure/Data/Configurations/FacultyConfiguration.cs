using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Data.Configurations;

public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.ToTable("faculty");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.EmployeeId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(f => f.Department)
            .HasMaxLength(100);

        builder.Property(f => f.Designation)
            .HasMaxLength(100);

        builder.Property(f => f.Qualification)
            .HasMaxLength(200);

        builder.Property(f => f.Specialization)
            .HasMaxLength(200);

        builder.Property(f => f.EmploymentType)
            .HasMaxLength(20);

        // Indexes
        builder.HasIndex(f => f.EmployeeId)
            .IsUnique()
            .HasDatabaseName("idx_faculty_employee_id");

        builder.HasIndex(f => f.UserId)
            .IsUnique()
            .HasDatabaseName("idx_faculty_user_id");

        builder.HasIndex(f => f.Department)
            .HasDatabaseName("idx_faculty_department");

        builder.HasIndex(f => f.IsActive)
            .HasDatabaseName("idx_faculty_is_active");
    }
}
