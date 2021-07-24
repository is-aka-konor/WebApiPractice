using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Api.ValidationFlow;
using WebApiPractice.Api.ValidationFlow.Interfaces;

namespace WebApiPractice.Api.Resources.Notes.Validations
{
    /// <summary>
    /// Describes the contract to validate a request with customer's note information
    /// </summary>
    public interface INoteValidationContract : IValidationContract
    {
        public string NoteText { get; set; }
    }

    /// <summary>
    /// This class does the actual validation of the request of type <see cref="INoteValidationContract"/>
    /// </summary>
    public class NoteValidationContractHandler : IValidationContractHandler
    {
        #region Constructor & private fields
        public NoteValidationContractHandler() { }
        #endregion

        public Task<List<ErrorMessage>> Handle(IValidationContract request, CancellationToken cancellationToken = default)
        {
            if (request is not INoteValidationContract contract)
            {
                var errorMessage = $"Validation Handler {nameof(NoteValidationContractHandler)}" +
                                   $" could not find contract: {nameof(INoteValidationContract)}";
                throw new Exception(errorMessage);
            }
            var messages = new List<ErrorMessage>();

            #region validations
            SharedValidationMethods.ValidateStringRequired(contract.NoteText, nameof(contract.NoteText), ref messages);
            SharedValidationMethods.ValidateStringLength(contract.NoteText, 1000, nameof(contract.NoteText), ref messages);
            #endregion

            return Task.FromResult(messages);
        }

        public bool AbortOnFailure() => false;
        public Type GetValidationContractType() => typeof(INoteValidationContract);
    }
}
