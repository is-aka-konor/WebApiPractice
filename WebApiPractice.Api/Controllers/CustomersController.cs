using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApiPractice.Api.Resources.Customer;

namespace WebApiPractice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CustomersController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> CreateCustomer([FromBody] PostCustomerRequest request)
        {
            var response = await this._mediator.Send(request).ConfigureAwait(false);
            return Created(new Uri(HttpContext.Request.GetDisplayUrl()), response);
        }

        [HttpGet]
        [Route("{customerId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCustomer([FromRoute] string customerId)
        {
            var request = new GetCustomerRequest()
            {
                CustomerExternalId = customerId
            };
            var response = await this._mediator.Send(request).ConfigureAwait(false);
            return Ok(response);
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetCustomers()
        {
            throw new NotImplementedException();
        }

        [HttpPatch]
        [Route("{customerId:Guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateCustomerStatus([FromRoute] Guid customerId, [FromBody] string status)
        {
            throw new NotImplementedException();
        }
    }
}
