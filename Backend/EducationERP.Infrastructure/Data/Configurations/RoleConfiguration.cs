using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RoleName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(255);

        // Indexes
        builder.HasIndex(r => r.RoleName)
            .IsUnique()
            .HasDatabaseName("idx_roles_role_name");

        // Relationships
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed default roles
        builder.HasData(
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                RoleName = "Admin",
                Description = "System Administrator",
                IsSystemRole = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                RoleName = "Faculty",
                Description = "Teaching Staff",
                IsSystemRole = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                RoleName = "Student",
                Description = "Enrolled Student",
                IsSystemRole = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                RoleName = "Parent",
                Description = "Student Guardian",
                IsSystemRole = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
