using Domain.Entities;
using MediatR;

namespace Application.Commands
{
    public record AccountsCommand(LoginModel login):IRequest<Employees>;
  
}
