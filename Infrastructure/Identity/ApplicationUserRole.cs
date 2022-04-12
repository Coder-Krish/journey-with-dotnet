using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationUserRole:IdentityUserRole<string>
{
    public ApplicationUser User { get; set; }
    public ApplicationRole Role { get; set; }
}