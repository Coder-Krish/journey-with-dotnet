using Application.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Commands;

public record CreateEmployeeCommand(EmployeeDTO employeeDTO) : IRequest<string>;