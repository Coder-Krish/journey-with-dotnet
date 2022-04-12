using Application.Queries;
using Domain.Entities;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.QueryHandlers;

public class GetEmployeesByIdQueryHandler : IRequestHandler<GetEmployeesByIdQuery, Employees>

{
    private readonly ApplicationDbContext _applicationDbContext;

    public GetEmployeesByIdQueryHandler(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Employees?> Handle(GetEmployeesByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _applicationDbContext.Employees.FirstOrDefaultAsync(
            a => a != null && a.EmployeeId == request.EmployeeId, cancellationToken: cancellationToken);
        return employee ?? null;
    }
}