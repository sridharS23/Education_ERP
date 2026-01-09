using EducationERP.Application.Common.Interfaces;
using EducationERP.Domain.Entities;
using EducationERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.Data;

public class DataSeeder
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DataSeeder(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        // Seed Roles
        await SeedRolesAsync();

        // Seed Permissions
        await SeedPermissionsAsync();

        // Seed Role Permissions
        await SeedRolePermissionsAsync();

        // Seed Default Admin User
        await SeedDefaultAdminAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] { "Admin", "Faculty", "Student", "Parent" };

        foreach (var roleName in roles)
        {
            var exists = await _context.Roles.AnyAsync(r => r.RoleName == roleName);
            if (!exists)
            {
                var role = Role.Create(roleName, $"{roleName} role", isSystemRole: true);
                _context.Roles.Add(role);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedPermissionsAsync()
    {
        var permissions = new[]
        {
            // User Management
            new { Name = "users.view", Description = "View users", Module = "Users" },
            new { Name = "users.create", Description = "Create users", Module = "Users" },
            new { Name = "users.edit", Description = "Edit users", Module = "Users" },
            new { Name = "users.delete", Description = "Delete users", Module = "Users" },

            // Student Management
            new { Name = "students.view", Description = "View students", Module = "Students" },
            new { Name = "students.create", Description = "Create students", Module = "Students" },
            new { Name = "students.edit", Description = "Edit students", Module = "Students" },
            new { Name = "students.delete", Description = "Delete students", Module = "Students" },

            // Faculty Management
            new { Name = "faculty.view", Description = "View faculty", Module = "Faculty" },
            new { Name = "faculty.create", Description = "Create faculty", Module = "Faculty" },
            new { Name = "faculty.edit", Description = "Edit faculty", Module = "Faculty" },
            new { Name = "faculty.delete", Description = "Delete faculty", Module = "Faculty" },

            // Role Management
            new { Name = "roles.view", Description = "View roles", Module = "Roles" },
            new { Name = "roles.create", Description = "Create roles", Module = "Roles" },
            new { Name = "roles.edit", Description = "Edit roles", Module = "Roles" },
            new { Name = "roles.delete", Description = "Delete roles", Module = "Roles" },
        };

        foreach (var perm in permissions)
        {
            var exists = await _context.Permissions.AnyAsync(p => p.PermissionName == perm.Name);
            if (!exists)
            {
                // Parse permission name to get resource and action (e.g., "users.view" -> resource="users", action="view")
                var parts = perm.Name.Split('.');
                var resource = parts.Length > 0 ? parts[0] : perm.Module;
                var action = parts.Length > 1 ? parts[1] : "unknown";
                
                var permission = Permission.Create(perm.Name, resource, action, perm.Description);
                _context.Permissions.Add(permission);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedRolePermissionsAsync()
    {
        var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
        if (adminRole == null) return;

        // Give admin all permissions
        var allPermissions = await _context.Permissions.ToListAsync();
        foreach (var permission in allPermissions)
        {
            var exists = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == adminRole.Id && rp.PermissionId == permission.Id);

            if (!exists)
            {
                var rolePermission = RolePermission.Create(adminRole.Id, permission.Id);
                _context.RolePermissions.Add(rolePermission);
            }
        }

        // Faculty permissions
        var facultyRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Faculty");
        if (facultyRole != null)
        {
            var facultyPermissions = new[] { "students.view", "students.edit" };
            foreach (var permName in facultyPermissions)
            {
                var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.PermissionName == permName);
                if (permission != null)
                {
                    var exists = await _context.RolePermissions
                        .AnyAsync(rp => rp.RoleId == facultyRole.Id && rp.PermissionId == permission.Id);

                    if (!exists)
                    {
                        var rolePermission = RolePermission.Create(facultyRole.Id, permission.Id);
                        _context.RolePermissions.Add(rolePermission);
                    }
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedDefaultAdminAsync()
    {
        var adminEmail = "admin@educationerp.com";
        var exists = await _context.Users.AnyAsync(u => u.Email == adminEmail);

        if (!exists)
        {
            // Create admin user using factory method
            var adminUser = User.Create(
                adminEmail,
                "System",
                "Administrator",
                "1234567890"
            );

            // Set password
            adminUser.SetPassword(_passwordHasher.HashPassword("Admin@123"));

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync();

            // Assign Admin role using factory method
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
            if (adminRole != null)
            {
                var userRole = UserRole.Create(adminUser.Id, adminRole.Id);
                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
            }
        }
    }
}
