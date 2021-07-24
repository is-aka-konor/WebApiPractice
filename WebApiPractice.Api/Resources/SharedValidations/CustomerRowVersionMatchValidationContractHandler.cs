using Microsoft.Extensions.Logging;
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
    public interface ICustomerRowVersionMatchValidationContract : IValidationContract
    {
        public string CustomerExternalId { get; set; }
        public string RowVersion { get; set; }
    }

    /// <summary>
    /// Handles validation of row version changes for customers
    /// </summary>
    public class CustomerRowVersionMatchValidationContractHandler : IValidationContractHandler
    {
        #region Constructor & private fields
        private readonly ICustomerRepository _repository;
        private readonly ILogger<CustomerRowVersionMatchValidationContractHandler> _logger;
        public CustomerRowVersionMatchValidationContractHandler(ICustomerRepository repository,
            ILogger<CustomerRowVersionMatchValidationContractHandler> logger)
        {
            this._repository = repository;
            this._logger = logger;
        }
        #endregion
        public async Task<List<ErrorMessage>> Handle(IValidationContract request, CancellationToken cancellationToken = default)
        {
            if (request is not ICustomerRowVersionMatchValidationContract contract)
            {
                var errorMessage = $"Validation Handler {nameof(CustomerRowVersionMatchValidationContractHandler)}" +
                                   $" could not find contract: {nameof(ICustomerRowVersionMatchValidationContract)}";
                throw new Exception(errorMessage);
            }
            var messages = new List<ErrorMessage>();

            var externalId = Guid.TryParse(contract.CustomerExternalId, out var guid) ? guid : Guid.Empty;
            if (externalId == Guid.Empty)
            {
                // If validation contracts were applied correctly then we should not be here
                _logger.LogWarning($"A request with the interface {nameof(ICustomerRowVersionMatchValidationContract)} with unrecognized Guid {contract.CustomerExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {contract.CustomerExternalId}");
            }
            var customer = await this._repository.GetCustomerByExternalId(externalId).ConfigureAwait(false);
            if(!string.IsNullOrWhiteSpace(contract.RowVersion)
                && !customer.RowVersion.Equals(contract.RowVersion, StringComparison.OrdinalIgnoreCase))
            {
                throw new ResourcePreconditionFailedException($"Resource with id:{contract.CustomerExternalId} has eTag {customer.RowVersion}. " +
                                                                $"Please provide this string as the If-Match header parameter" );
            }
            return messages;
        }

        public bool AbortOnFailure() => true;
        public Type GetValidationContractType() => typeof(ICustomerRowVersionMatchValidationContract);
    }
}
