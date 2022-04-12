using Application.Queries;
using Domain.Entities;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.QueryHandlers;

public class GetEmployeesQueryHandler:IRequestHandler<GetEmployeesQuery, List<Employees>>
{
    private readonly ApplicationDbContext _applicationDbContext;

    public GetEmployeesQueryHandler(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public async Task<List<Employees>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        return await _applicationDbContext.Employees.ToListAsync<Employees>(cancellationToken: cancellationToken);
    }
}