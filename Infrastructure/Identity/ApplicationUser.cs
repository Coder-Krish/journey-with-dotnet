using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationUser:IdentityUser
{
    public UserType UserType { get; set; }
    public string Fullname { get; set; }
    public bool IsActive { get; set; }
    public Guid EmployeesId { get; set; }
    public Employees Employees { get; set; }
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
    public ICollection<ApplicationRole> Roles { get; set; }
}