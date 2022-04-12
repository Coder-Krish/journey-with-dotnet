using Domain.Entities;
using MediatR;

namespace Application.Commands;

public record CreateEmployeeCommand(Employees _employees) : IRequest<Employees>;