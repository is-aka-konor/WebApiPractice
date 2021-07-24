using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customers.Validations;
using WebApiPractice.Api.Resources.Notes.Validations;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Api.Resources.Notes
{
    public class GetNoteRequest : IRequest<GetNoteResponse>
        , ICustomerNotFoundValidationContract
        , INoteNotFoundValidationContract
    {
        public string CustomerExternalId { get; set; } = string.Empty;
        public string NoteExternalId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Describes a model of the response to <see cref="GetNoteRequest"/>
    /// </summary>
    public class GetNoteResponse
    {
        public string NoteExternalId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string NoteText { get; set; } = string.Empty;
        [JsonIgnore]
        public string RowVersion { get; set; } = string.Empty;
    }
    public class GetNoteHandler : IRequestHandler<GetNoteRequest, GetNoteResponse>
    {
        #region Private fields and constructor
        private readonly AppDbContext _appDbContext;
        private readonly IObjectMapper _mapper;
        private readonly ILogger<GetNoteHandler> _logger;
        public GetNoteHandler(AppDbContext appDbContext,
            IObjectMapper mapper,
            ILogger<GetNoteHandler> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion

        public async Task<GetNoteResponse> Handle(GetNoteRequest request, CancellationToken cancellationToken)
        {
            var externalId = Guid.TryParse(request.NoteExternalId, out var guid) ? guid : Guid.Empty;
            if (externalId == Guid.Empty)
            {
                // If validation contracts were applied correctly then we should not be here
                _logger.LogWarning($"A get note request with unrecognized Guid {request.NoteExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {request.NoteExternalId}");
            }
            var note = await _appDbContext.Notes.FirstOrDefaultAsync(x => x.NoteExternalId.Equals(externalId), cancellationToken).ConfigureAwait(false);
            return _mapper.Map<Note, GetNoteResponse>(note);
        }
    }
}