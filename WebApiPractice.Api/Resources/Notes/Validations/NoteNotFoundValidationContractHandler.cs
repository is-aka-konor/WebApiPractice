using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Api.ValidationFlow;
using WebApiPractice.Api.ValidationFlow.Interfaces;
using WebApiPractice.Persistent.Context;

namespace WebApiPractice.Api.Resources.Notes.Validations
{
    /// <summary>
    /// Describes the contract to validate a request of type <see cref="GetCustomerRequest"/>
    /// </summary>
    public interface INoteNotFoundValidationContract : IValidationContract
    {
        public string NoteExternalId { get; set; }
    }

    /// <summary>
    /// This class does the actual validation of the request of type <see cref="GetCustomerRequest"/>
    /// </summary>
    public class NoteNotFoundValidationContractHandler : IValidationContractHandler
    {
        #region Constructor & private fields
        private readonly AppDbContext _appDbContext;
        public NoteNotFoundValidationContractHandler(AppDbContext context)
        {
            this._appDbContext = context;
        }
        #endregion

        public async Task<List<ErrorMessage>> Handle(IValidationContract request, CancellationToken cancellationToken = default)
        {
            if (request is not INoteNotFoundValidationContract contract)
            {
                var errorMessage = $"Validation Handler {nameof(NoteNotFoundValidationContractHandler)}" +
                                   $" could not find contract: {nameof(INoteNotFoundValidationContract)}";
                throw new Exception(errorMessage);
            }
            var messages = new List<ErrorMessage>();

            #region Required validations
            var externalFieldName = "NoteId";
            SharedValidationMethods.ValidateStringRequired(contract.NoteExternalId, externalFieldName, ref messages);
            if (!SharedValidationMethods.IsValidExternalId(contract.NoteExternalId, externalFieldName, out var externalClientGuid, ref messages))
            {
                return messages;
            }
            #endregion

            #region Id Check
            var isExistingNote = await this._appDbContext
                                                .Notes.AnyAsync(m => m.NoteExternalId.Equals(externalClientGuid), cancellationToken)
                                                .ConfigureAwait(false);
            if (!isExistingNote)
            {
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {contract.NoteExternalId}");
            }
            #endregion
            return messages;
        }

        public bool AbortOnFailure() => true;
        public Type GetValidationContractType() => typeof(INoteNotFoundValidationContract);
    }
}
