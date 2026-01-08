using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Data.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(ur => ur.Id);

        // Composite unique index
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique()
            .HasDatabaseName("idx_user_roles_user_role");

        builder.HasIndex(ur => ur.UserId)
            .HasDatabaseName("idx_user_roles_user_id");

        builder.HasIndex(ur => ur.RoleId)
            .HasDatabaseName("idx_user_roles_role_id");
    }
}
