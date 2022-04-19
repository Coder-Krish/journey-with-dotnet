using Application.Common.interfaces;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.QueryHandlers;

public class GetEmployeesByIdQueryHandler : IRequestHandler<GetEmployeesByIdQuery, Employees>

{
    private readonly IApplicationDbContext _applicationDbContext;

    public GetEmployeesByIdQueryHandler(IApplicationDbContext applicationDbContext)
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