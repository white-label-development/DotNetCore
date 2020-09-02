using InvoiceManagementApp.Application.Common.Interfaces;
using InvoiceManagementApp.Application.Invoices.Commands;
using InvoiceManagementApp.Application.Invoices.Queries;
using InvoiceManagementApp.Application.Invoices.VIewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvoiceManagementApp.Controllers
{
    [Authorize]
    public class InvoicesController : ApiController
    {

        private readonly ICurrentUserService _currentUserService;

        public InvoicesController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateInvoiceCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet]
        public async Task<IList<InvoiceVm>> Get()
        {
            return await Mediator.Send(new GetUserInvoicesQuery { User = _currentUserService.UserId });
        }
    }
}
