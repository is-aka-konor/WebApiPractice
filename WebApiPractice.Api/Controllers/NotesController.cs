using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApiPractice.Api.Domain;
using WebApiPractice.Api.Resources.Notes;

namespace WebApiPractice.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class NotesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ApiConfiguration _apiConfiguration;
        public NotesController(IMediator mediator
            , IOptions<ApiConfiguration> options)
        {
            this._mediator = mediator;
            this._apiConfiguration = options.Value;
        }

        [HttpPost("{customerId}/[controller]")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateNote([FromRoute] string customerId, [FromBody] PostNoteRequest request)
        {
            request.CustomerExternalId = customerId;
            var response = await this._mediator.Send(request).ConfigureAwait(false);
            HttpContext.Response.Headers.Add(HeaderNames.ETag, response.RowVersion);
            return Created(new Uri(HttpContext.Request.GetDisplayUrl()), response);
        }

        [HttpGet]
        [Route("{customerId}/[controller]/{noteId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetNotes([FromRoute] string customerId, [FromRoute] string noteId)
        {
            var request = new GetNoteRequest()
            {
                CustomerExternalId = customerId,
                NoteExternalId = noteId
            };
            var response = await this._mediator.Send(request).ConfigureAwait(false);
            var eTag = response.RowVersion;
            HttpContext.Response.Headers.Add(HeaderNames.ETag, eTag);
            if (HttpContext.Request.Headers.ContainsKey(HeaderNames.IfMatch))
            {
                var incomingEtag = HttpContext.Request.Headers[HeaderNames.IfMatch].ToString();
                // if both the etags are equal
                // raise a 304 Not Modified Response
                if (incomingEtag.Equals(eTag))
                {
                    return new StatusCodeResult((int)HttpStatusCode.NotModified);
                }
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("{customerId}/[controller]")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCustomers(
            [FromRoute] string customerId,
            [FromQuery] int limit,
            [FromQuery] string? nextCursor = "")
        {
            var request = new GetNotesRequest()
            {
                CustomerExternalId = customerId,
                NextCursor = nextCursor!,
                Limit = (limit > 0 && limit < this._apiConfiguration.ResponseMaxLimit
                            ? limit
                            : this._apiConfiguration.ResponseMaxLimit)
            };
            var response = await this._mediator.Send(request).ConfigureAwait(false);
            return Ok(response);
        }
    }
}
