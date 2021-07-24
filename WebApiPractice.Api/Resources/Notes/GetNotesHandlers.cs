using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.Extensions;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customers.Validations;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Api.Resources.Notes
{
    /// <summary>
    /// Describes a model of incoming request to get customers' note
    /// </summary>
    public class GetNotesRequest : IRequest<GetNotesResponse>
        , ICustomerNotFoundValidationContract
    {
        [JsonIgnore]
        public string CustomerExternalId { get; set; } = string.Empty;
        public string NextCursor { get; set; } = string.Empty;
        public int Limit { get; set; }
    }

    /// <summary>
    /// Describes a model of the response to <see cref="GetNotesRequest"/>
    /// </summary>
    public class GetNotesResponse
    {
        public List<GetNoteResponse> Notes { get; set; }
        public ResponseMetadata ResponseMetadata { get; set; }

        public GetNotesResponse()
        {
            Notes = new List<GetNoteResponse>();
            ResponseMetadata = new ResponseMetadata();
        }
    }

    public class GetNotesHandlers : IRequestHandler<GetNotesRequest, GetNotesResponse>
    {
        #region Private fields and constructor
        private readonly AppDbContext _appDbContext;
        private readonly IObjectMapper _mapper;
        private readonly ILogger<GetNotesHandlers> _logger;
        public GetNotesHandlers(AppDbContext appDbContext,
            IObjectMapper mapper,
            ILogger<GetNotesHandlers> logger)
        {
            this._appDbContext = appDbContext;
            this._logger = logger;
            this._mapper = mapper;
        }
        #endregion
        public async Task<GetNotesResponse> Handle(GetNotesRequest request, CancellationToken cancellationToken)
        {
            var response = new GetNotesResponse();
            var cursor = request.NextCursor.Base64DecodeInt();
            var externalId = Guid.TryParse(request.CustomerExternalId, out var guid) ? guid : Guid.Empty;
            if (externalId == Guid.Empty)
            {
                // If validation contracts were applied correctly then we should not be here
                _logger.LogWarning($"A get customer request with unrecognized Guid {request.CustomerExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {request.CustomerExternalId}");
            }
            var query = this._appDbContext.Notes.AsNoTracking()
                .Where(r => r.Customer.CustomerExternalId.Equals(externalId)
                            && r.NoteId >= cursor);

            var notes = await query.OrderBy(c => c.CreatedAt)
                                        .Select(c => c)
                                        .ToListAsync(request.Limit,
                                        (note) =>
                                        {
                                            response.ResponseMetadata.HasNext = true;
                                            response.ResponseMetadata.NextCursor = note.NoteId.Base64Encode();
                                        }, cancellationToken)
                                        .ConfigureAwait(false);
            response.Notes = this._mapper.Map<Note, GetNoteResponse>(notes).ToList();
            return response;
        }
    }
}
