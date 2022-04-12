using Domain.Entities;
using MediatR;

namespace Application.Queries;

public record GetEmployeesQuery : IRequest<List<Employees>>;
