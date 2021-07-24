using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.Resources.Customers.Validations;
using WebApiPractice.Api.Resources.Notes.Validations;
using WebApiPractice.Api.Resources.SharedValidations;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Persistent.Repositories;

namespace WebApiPractice.Api.Resources.Notes
{
    /// <summary>
    /// Describes a model of incoming request to Delete customer's note
    /// </summary>
    public class DeleteNoteRequest : IRequest<DeleteNoteResponse>
        , ICustomerNotFoundValidationContract
        , INoteNotFoundValidationContract
        , INoteRowVersionMatchValidationContract
    {
        [JsonIgnore]
        public string ExternalId => NoteExternalId;
        [JsonIgnore]
        public string CustomerExternalId { get; set; } = string.Empty;
        [JsonIgnore]
        public string NoteExternalId { get; set; } = string.Empty;
        [JsonIgnore]
        public string RowVersion { get; set; } = string.Empty;
    }

    /// <summary>
    /// Describes a model of the response to <see cref="DeleteNoteRequest"/>
    /// </summary>
    public class DeleteNoteResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class DeleteNoteHandler : IRequestHandler<DeleteNoteRequest, DeleteNoteResponse>
    {
        #region Private fields and constructor
        private readonly ILogger<DeleteNoteHandler> _logger;
        private readonly INoteRepository _noteRepository;
        public DeleteNoteHandler(INoteRepository noteRepository
                , ILogger<DeleteNoteHandler> logger)
        {
            this._noteRepository = noteRepository;
            this._logger = logger;
        }
        #endregion

        public async Task<DeleteNoteResponse> Handle(DeleteNoteRequest request, CancellationToken cancellationToken)
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
            await this._noteRepository.DeleteNote(note, cancellationToken).ConfigureAwait(false);
            return new DeleteNoteResponse() { Message = $"A note with id {request.ExternalId} successfully deleted." };
        }
    }
}
