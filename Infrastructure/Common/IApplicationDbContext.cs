using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> ApplicationUsers { get; set; }
    DbSet<ApplicationRole> ApplicationRoles { get; set; }
    DbSet<Employees?> Employees { get; set; }
}