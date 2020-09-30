using InvoiceManagementApp.Application.Products.Commands;
using InvoiceManagementApp.Application.Products.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InvoiceManagementApp.Controllers
{

    // [Authorize]
    public class ProductController : ApiController
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllProductsQuery()));
        }
    }
}
