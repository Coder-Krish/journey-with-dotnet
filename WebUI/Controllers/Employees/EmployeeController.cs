using Application.Commands;
using Application.DTOs;
using Application.Queries;
using Domain.Constants;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebUI.RealTime.Hubs;

namespace WebUI.Controllers
{
    public class EmployeeController : Controller
    {
        private IMediator _mediator;
        private readonly IHubContext<NotificationHub> _hubContext;

        //protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        public EmployeeController(IMediator mediator, IHubContext<NotificationHub> hubContext)
        {
            this._mediator = mediator;
            _hubContext = hubContext;
        }

        /// <summary>
        /// List out all the employees
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        [Route("GetEmployees")]
        [Authorize(Policy = Policies.AdminPolicy)]
        public async Task<List<Employees>> GetPersons()
        {
            return await _mediator.Send(new GetEmployeesQuery());
        }

        /// <summary>
        /// Gets the Employees by its Guid...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetEmployeesById/{id}")]
        [Authorize(Policy = Policies.PublicPolicy)]
        public async Task<Employees> GetEmployeesById(Guid id)
        {
            return await _mediator.Send(new GetEmployeesByIdQuery(id));
        }

        /// <summary>
        /// Adds Employees with their informations...
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddEmployees")]
        public async Task AddEmployees([FromBody] EmployeeDTO employeeDTO)
        {
            var result = await _mediator.Send(new CreateEmployeeCommand(employeeDTO));
            await _hubContext.Clients.All.SendAsync(RealTimeConstants.Notification, "New User Added Successfully in the System.");
        }
    }
}
