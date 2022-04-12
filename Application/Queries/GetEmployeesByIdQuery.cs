using Domain.Entities;
using MediatR;

namespace Application.Queries;

public record GetEmployeesByIdQuery(Guid EmployeeId):IRequest<Employees>;