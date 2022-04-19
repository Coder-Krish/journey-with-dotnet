using Application.Common.Identities;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Application.Common.interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        DbSet<ApplicationRole> ApplicationRoles { get; set; }
        DbSet<Employees?> Employees { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
