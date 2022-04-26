using Application.Commands;
using Application.Common.Identities;
using Application.Common.interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.CommandHandlers;

public class CreateEmployeeCommandHandler:IRequestHandler<CreateEmployeeCommand, string>
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public CreateEmployeeCommandHandler(IApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _applicationDbContext = applicationDbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task<string> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employees = request.employeeDTO;
        Employees employee = new Employees()
        {
            FullName = employees.FullName,
            Email = employees.Email,
            Address = employees.Address,
            Password = employees.Password
        };
        _applicationDbContext.Employees.Add(employee);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);
        
        List<ApplicationRole> identityRoles =  _applicationDbContext.ApplicationRoles.ToList();
        var normalUserRole = identityRoles.FirstOrDefault(a => String.Equals(a.Name, AppRoles.NormalUser, StringComparison.CurrentCultureIgnoreCase));
        var normalUser = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = employees.FullName.Substring(0, employee.FullName.IndexOf(" ", StringComparison.Ordinal)), EmployeesId = employee.EmployeeId, Fullname = employee.FullName, Email = employee.Email, UserType = UserType.NormalUser, IsActive = true };
        await _userManager.CreateAsync(normalUser, employees.Password);
        await _userManager.AddToRolesAsync(normalUser, new[] { normalUserRole?.Name });
        return "User Added Successfully.";
    }
}