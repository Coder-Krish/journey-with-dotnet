using Application.Commands;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers.UserAccounts
{
    public class AccountsController : Controller
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<Employees> Login([FromBody] LoginModel login)
        {
            //throw new Exception("Hello Logger , this is exception thrown from AccountController");
            return await _mediator.Send(new AccountsCommand(login));
        }
    }
}
