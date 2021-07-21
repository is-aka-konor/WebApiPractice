using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiPractice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{customerId:Guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCustomer([FromRoute] Guid customerId)
        {
            throw new NotImplementedException();
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
