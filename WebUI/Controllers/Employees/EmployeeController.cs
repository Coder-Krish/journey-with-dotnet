using Application.Commands;
using Application.DTOs;
using Application.Queries;
using Domain.Constants;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    public class EmployeeController : Controller
    {
        private IMediator _mediator;
        //protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        public EmployeeController(IMediator mediator)
        {
            this._mediator = mediator;
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
        public async Task<Employees> AddEmployees([FromBody] EmployeeDTO employeeDTO)
        {
            return await _mediator.Send(new CreateEmployeeCommand(employeeDTO));
        }
    }
}
