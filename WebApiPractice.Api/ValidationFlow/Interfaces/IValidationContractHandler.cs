using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.ResponseStructure;

namespace WebApiPractice.Api.ValidationFlow.Interfaces
{
    /// <summary>
    /// The handler for all IValidationContracts. The IValidationContracts exposes the fields required by validations,
    /// and this interface handles the code that runs when validation is executed.
    /// </summary>
    public interface IValidationContractHandler
    {
        /// <summary>
        /// This method must implement the code required to handle the validation
        /// </summary>
        /// <param name="request">Accepts <see cref="IValidationContract"/> type as object.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        Task<List<ErrorMessage>> Handle(IValidationContract request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether the subsequent validation handlers to aborted when this handler fails.
        /// </summary>
        bool AbortOnFailure();

        /// <summary>
        /// The type of IValidationContract that this handler is for.
        /// </summary>
        Type GetValidationContractType();
    }
}
