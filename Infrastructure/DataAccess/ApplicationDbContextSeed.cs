using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Duende.IdentityServer.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.DataAccess;

public static class ApplicationDbContextSeed
{
    public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
    {
        if (!roleManager.Roles.Any())
        {
            var adminEmployee = new Employees()
            {
                FullName = "admin admin",
                Email = Email.From("admin@localhost.com"),
                Address = "Sample Address"
            };

            context.Employees.Add(adminEmployee);

            await context.SaveChangesAsync();


            var adminRole = new ApplicationRole { Id = Guid.NewGuid().ToString(), Name = AppRoles.Admin, Type = RoleType.AdminRole };
            await roleManager.CreateAsync(adminRole);


            var normalUser = new ApplicationRole { Id = Guid.NewGuid().ToString(), Name = AppRoles.NormalUser, Type = RoleType.UserRole };
            await roleManager.CreateAsync(normalUser);

            var adminUser = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "admin", EmployeesId = adminEmployee.EmployeeId, Fullname = adminEmployee.FullName, Email = adminEmployee.Email, UserType = UserType.AdminUser, IsActive = true };
            await userManager.CreateAsync(adminUser, "P@ssw0rd");
            await userManager.AddToRolesAsync(adminUser, new[] { adminRole.Name });
            await userManager.AddToRolesAsync(adminUser, new[] { normalUser.Name });
        }
    }
}