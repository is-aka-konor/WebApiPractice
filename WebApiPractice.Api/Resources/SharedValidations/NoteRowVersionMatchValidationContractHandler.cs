﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Api.ValidationFlow.Interfaces;
using WebApiPractice.Persistent.Repositories;

namespace WebApiPractice.Api.Resources.SharedValidations
{
    /// <summary>
    /// Describes the contract to validate a request with customer information
    /// </summary>
    public interface INoteRowVersionMatchValidationContract : IValidationContract
    {
        public string NoteExternalId { get; set; }
        public string RowVersion { get; set; }
    }

    // Ideally this validation should be done through a generic approach but don't have time for it
    /// <summary>
    /// Handles validation of row version changes for notes
    /// </summary>
    public class NoteRowVersionMatchValidationContractHandler : IValidationContractHandler
    {
        #region Constructor & private fields
        private readonly INoteRepository _repository;
        private readonly ILogger<NoteRowVersionMatchValidationContractHandler> _logger;
        public NoteRowVersionMatchValidationContractHandler(INoteRepository repository,
            ILogger<NoteRowVersionMatchValidationContractHandler> logger)
        {
            this._repository = repository;
            this._logger = logger;
        }
        #endregion
        public async Task<List<ErrorMessage>> Handle(IValidationContract request, CancellationToken cancellationToken = default)
        {
            if (request is not INoteRowVersionMatchValidationContract contract)
            {
                var errorMessage = $"Validation Handler {nameof(NoteRowVersionMatchValidationContractHandler)}" +
                                   $" could not find contract: {nameof(INoteRowVersionMatchValidationContract)}";
                throw new Exception(errorMessage);
            }
            var messages = new List<ErrorMessage>();

            var externalId = Guid.TryParse(contract.NoteExternalId, out var guid) ? guid : Guid.Empty;
            if (externalId == Guid.Empty)
            {
                // If validation contracts were applied correctly then we should not be here
                _logger.LogWarning($"A request with the interface {nameof(INoteRowVersionMatchValidationContract)} with unrecognized Guid {contract.NoteExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {contract.NoteExternalId}");
            }
            var customer = await this._repository.GetNoteByExternalId(externalId, cancellationToken).ConfigureAwait(false);
            if (! string.IsNullOrWhiteSpace(contract.RowVersion)
                && !customer.RowVersion.Equals(contract.RowVersion, StringComparison.OrdinalIgnoreCase))
            {
                throw new ResourcePreconditionFailedException($"Resource with id:{contract.NoteExternalId} has eTag: {customer.RowVersion}. " +
                                                               "Please provide this string as the If-Match header parameter");
            }
            return messages;
        }

        public bool AbortOnFailure() => true;
        public Type GetValidationContractType() => typeof(INoteRowVersionMatchValidationContract);
    }
}
