using Application.Commands;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.DataAccess;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CommandHandlers;

public class CreateEmployeeCommandHandler:IRequestHandler<CreateEmployeeCommand, Employees>
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public CreateEmployeeCommandHandler(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _applicationDbContext = applicationDbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task<Employees> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employees = request._employees;
        _applicationDbContext.Add<Employees>(employees);
        await _applicationDbContext.SaveChangesAsync();
        
        List<ApplicationRole> identityRoles =  _applicationDbContext.ApplicationRoles.ToList();
        var normalUserRole = identityRoles.FirstOrDefault(a => String.Equals(a.Name, AppRoles.NormalUser, StringComparison.CurrentCultureIgnoreCase));
        var normalUser = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = employees.FullName.Substring(0, employees.FullName.IndexOf(" ", StringComparison.Ordinal)), EmployeesId = employees.EmployeeId, Fullname = employees.FullName, Email = employees.Email, UserType = UserType.NormalUser, IsActive = true };
        await _userManager.CreateAsync(normalUser, employees.Password);
        await _userManager.AddToRolesAsync(normalUser, new[] { normalUserRole?.Name });
        return employees;
    }
}