using MediatR;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customers.Validations;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Persistent.DataModels;
using WebApiPractice.Persistent.Repositories;

namespace WebApiPractice.Api.Resources.Notes
{
    /// <summary>
    /// Describes a model of incoming request to create a note for a customer
    /// </summary>
    public class PostNoteRequest : IRequest<PostNoteResponse>
        , ICustomerNotFoundValidationContract
    {
        [JsonIgnore]
        public string CustomerExternalId { get; set; } = string.Empty;
        [JsonIgnore]
        public DateTime CreatedAt => DateTime.Now;
        public string NoteText { get; set; } = string.Empty;
    }

    /// <summary>
    /// Describes a model of the response to <see cref="PostNoteRequest"/>
    /// </summary>
    public class PostNoteResponse : GetNoteResponse
    {
    }

    /// <summary>
    /// Handles the creation of a new customer
    /// </summary>
    public class PostNoteHandler : IRequestHandler<PostNoteRequest, PostNoteResponse>
    {
        #region Private fields and constructor
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<PostNoteHandler> _logger;
        private readonly INoteRepository _noteRepository;
        private readonly IObjectMapper _mapper;
        public PostNoteHandler(ICustomerRepository customerRepository
                , INoteRepository noteRepository
                , IObjectMapper mapper
                , ILogger<PostNoteHandler> logger)
        {
            this._customerRepository = customerRepository;
            this._noteRepository = noteRepository;
            this._mapper = mapper;
            this._logger = logger;
        }
        #endregion

        public async Task<PostNoteResponse> Handle(PostNoteRequest request, CancellationToken cancellationToken)
        {
            var externalId = Guid.TryParse(request.CustomerExternalId, out var guid) ? guid : Guid.Empty;
            if (externalId == Guid.Empty)
            {
                // If validation contracts were applied correctly then we should not be here
                _logger.LogWarning($"A post note request with unrecognized Customer Guid {request.CustomerExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {request.CustomerExternalId}");
            }
            var customer = await this._customerRepository.GetCustomerByExternalId(externalId, cancellationToken).ConfigureAwait(false);
            var note = this._mapper.Map<PostNoteRequest, Note>(request);
            note.Customer = customer;
            note = await this._noteRepository.SaveNote(note, cancellationToken).ConfigureAwait(false);
            return this._mapper.Map<Note, PostNoteResponse>(note);
        }
    }
}
