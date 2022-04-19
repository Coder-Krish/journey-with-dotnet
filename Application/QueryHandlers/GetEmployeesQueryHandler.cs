using Application.Common.interfaces;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.QueryHandlers;

public class GetEmployeesQueryHandler:IRequestHandler<GetEmployeesQuery, List<Employees>>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public GetEmployeesQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public async Task<List<Employees>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.Employees.ToListAsync<Employees>(cancellationToken: cancellationToken);
    }
}