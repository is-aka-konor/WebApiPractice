using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customers.Validations;
using WebApiPractice.Api.Resources.Notes.Validations;
using WebApiPractice.Api.Resources.SharedValidations;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Persistent.DataModels;
using WebApiPractice.Persistent.Repositories;

namespace WebApiPractice.Api.Resources.Notes
{
    /// <summary>
    /// Describes a model of incoming request to update customer's note
    /// </summary>
    public class UpdateNoteRequest : IRequest<UpdateNoteResponse>
        , ICustomerNotFoundValidationContract
        , INoteNotFoundValidationContract
        , INoteRowVersionMatchValidationContract
        , INoteValidationContract
    {
        [JsonIgnore]
        public string ExternalId => NoteExternalId;
        [JsonIgnore]
        public string CustomerExternalId { get; set; } = string.Empty;
        [JsonIgnore]
        public string NoteExternalId { get; set; } = string.Empty;
        [JsonIgnore]
        public string RowVersion { get; set; } = string.Empty;
        public string NoteText { get; set; } = string.Empty;
    }

    /// <summary>
    /// Describes a model of the response to <see cref="UpdateNoteRequest"/>
    /// </summary>
    public class UpdateNoteResponse : PostNoteResponse { }

    /// <summary>
    /// Handles update customer's note requests of type <see cref="UpdateNoteRequest"/>
    /// </summary>
    public class UpdateNoteHandler : IRequestHandler<UpdateNoteRequest, UpdateNoteResponse>
    {
        #region Private fields and constructor
        private readonly ILogger<UpdateNoteHandler> _logger;
        private readonly INoteRepository _noteRepository;
        private readonly IObjectMapper _mapper;
        public UpdateNoteHandler(INoteRepository noteRepository
                , IObjectMapper mapper
                , ILogger<UpdateNoteHandler> logger)
        {
            this._noteRepository = noteRepository;
            this._mapper = mapper;
            this._logger = logger;
        }
        #endregion

        public async Task<UpdateNoteResponse> Handle(UpdateNoteRequest request, CancellationToken cancellationToken)
        {
            var noteExternalId = Guid.TryParse(request.NoteExternalId, out var noteGuid) ? noteGuid : Guid.Empty;
            if (noteExternalId == Guid.Empty)
            {
                // If validation contracts were applied correctly then we should not be here
                _logger.LogWarning($"A update note request with unrecognized Guid {request.NoteExternalId} note external id by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {request.NoteExternalId}");
            }
            var note = await this._noteRepository.GetNoteByExternalId(noteExternalId, cancellationToken).ConfigureAwait(false);
            if (note is null)
            {
                // If validation contracts were applied correctly then we should not be here
                this._logger.LogWarning($"An update note request with note external id: {request.NoteExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {request.NoteExternalId}");
            }

            note.NoteText = request.NoteText;
            note = await this._noteRepository.UpdateNote(note, cancellationToken).ConfigureAwait(false);
            return this._mapper.Map<Note, UpdateNoteResponse>(note);
        }
    }
}
