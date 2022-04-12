using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationRole:IdentityRole<string>
{
    public RoleType Type { get; set; }
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
    public ICollection<ApplicationUser> User { get; set; }
}