using Microsoft.AspNetCore.Identity;

namespace Application.Common.Identities;
public class ApplicationUserRole:IdentityUserRole<string>
{
    public ApplicationUser User { get; set; }
    public ApplicationRole Role { get; set; }
}