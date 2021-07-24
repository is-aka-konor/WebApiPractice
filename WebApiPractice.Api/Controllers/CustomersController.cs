using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WebApiPractice.Api.Domain;
using WebApiPractice.Api.Resources.Customers;

namespace WebApiPractice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ApiConfiguration _apiConfiguration;
        public CustomersController(IMediator mediator
            , IOptions<ApiConfiguration> options)
        {
            this._mediator = mediator;
            this._apiConfiguration = options.Value;
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
        public async Task<IActionResult> GetCustomers(
            [FromQuery] int limit,
            [FromQuery] string? nextCursor = "",
            [FromQuery] string? status = "",
            [FromQuery] string? firstName = "",
            [FromQuery] string? lastName = "")
        {
            var request = new GetCustomersRequest()
            {
                NextCursor = nextCursor!,
                Limit = (limit > 0 && limit < this._apiConfiguration.ResponseMaxLimit
                            ? limit
                            : this._apiConfiguration.ResponseMaxLimit),
                Status = status!,
                FirstName = firstName!,
                LastName = lastName!
            };
            var response = await this._mediator.Send(request).ConfigureAwait(false);
            return Ok(response);
        }

        [HttpPut]
        [Route("{customerId}")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateCustomerStatus([FromRoute] string customerId, [FromBody] UpdateCustomerRequest request)
        {
            request.CustomerExternalId = customerId;
            var response = await this._mediator.Send(request).ConfigureAwait(false);
            return Ok(response);
        }
    }
}
