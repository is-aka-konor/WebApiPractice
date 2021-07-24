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

namespace WebApiPractice.Api.Resources.Customers.Validations
{
    /// <summary>
    /// Describes the contract to validate a request of type <see cref="GetCustomerRequest"/>
    /// </summary>
    public interface ICustomerNotFoundValidationContract : IValidationContract
    {
        public string ExternalId { get; set; }
    }

    /// <summary>
    /// This class does the actual validation of the request of type <see cref="GetCustomerRequest"/>
    /// </summary>
    public class CustomerNotFoundValidationContractHandler : IValidationContractHandler
    {
        #region Constructor & private fields
        private readonly AppDbContext _appDbContext;
        public CustomerNotFoundValidationContractHandler(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }
        #endregion

        public async Task<List<ErrorMessage>> Handle(IValidationContract request, CancellationToken cancellationToken = default)
        {
            if (!(request is ICustomerNotFoundValidationContract contract))
            {
                var errorMessage = $"Validation Handler {nameof(CustomerNotFoundValidationContractHandler)}" +
                                   $" could not find contract: {nameof(ICustomerNotFoundValidationContract)}";
                throw new Exception(errorMessage);
            }
            var messages = new List<ErrorMessage>();

            #region Required validations
            var externalFieldName = "CustomerId";
            SharedValidationMethods.ValidateStringRequired(contract.ExternalId, externalFieldName, ref messages);
            if(!SharedValidationMethods.IsValidExternalId(contract.ExternalId, externalFieldName, out var externalClientGuid, ref messages))
            {
                return messages;
            }
            #endregion

            #region Id Check
            var isExistingCustomer = await this._appDbContext
                                            .Customers
                                            .AnyAsync(m => m.CustomerExternalId.Equals(externalClientGuid), cancellationToken)
                                            .ConfigureAwait(false);
            if (!isExistingCustomer)
            {
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {contract.ExternalId}");
            }
            #endregion
            return messages;
        }

        public bool AbortOnFailure() => true;
        public Type GetValidationContractType() => typeof(ICustomerNotFoundValidationContract);
    }
}
