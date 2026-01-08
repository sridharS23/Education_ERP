using EducationERP.Domain.Entities;
using EducationERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.AdmissionNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.BloodGroup)
            .HasMaxLength(5);

        builder.Property(s => s.Address)
            .HasMaxLength(500);

        builder.Property(s => s.City)
            .HasMaxLength(100);

        builder.Property(s => s.State)
            .HasMaxLength(100);

        builder.Property(s => s.PostalCode)
            .HasMaxLength(20);

        builder.Property(s => s.Country)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("India");

        builder.Property(s => s.EmergencyContactName)
            .HasMaxLength(200);

        builder.Property(s => s.EmergencyContactPhone)
            .HasMaxLength(20);

        builder.Property(s => s.EnrollmentStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.AcademicYear)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(s => s.Gender)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Indexes
        builder.HasIndex(s => s.AdmissionNumber)
            .IsUnique()
            .HasDatabaseName("idx_students_admission_number");

        builder.HasIndex(s => s.UserId)
            .IsUnique()
            .HasDatabaseName("idx_students_user_id");

        builder.HasIndex(s => s.EnrollmentStatus)
            .HasDatabaseName("idx_students_enrollment_status");

        builder.HasIndex(s => s.AcademicYear)
            .HasDatabaseName("idx_students_academic_year");
    }
}
