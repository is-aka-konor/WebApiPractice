using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Api.ValidationFlow;
using WebApiPractice.Api.ValidationFlow.Interfaces;

namespace WebApiPractice.Api.Resources.Customer.Validations
{
    /// <summary>
    /// Describes the contract to validate a request with customer information
    /// </summary>
    public interface ICustomerInformationValidationContract : IValidationContract
    {
        public CustomerStatus Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    /// <summary>
    /// This class does the actual validation of the request of type <see cref="ICustomerInformationValidationContract"/>
    /// </summary>
    public class CustomerInformationValidationHandler : IValidationContractHandler
    {
        #region Constructor & private fields
        public CustomerInformationValidationHandler() { }
        #endregion

        public Task<List<ErrorMessage>> Handle(IValidationContract request, CancellationToken cancellationToken = default)
        {
            if (!(request is ICustomerInformationValidationContract contract))
            {
                var errorMessage = $"Validation Handler {nameof(CustomerInformationValidationHandler)}" +
                                   $" could not find contract: {nameof(ICustomerInformationValidationContract)}";
                throw new Exception(errorMessage);
            }
            var messages = new List<ErrorMessage>();

            #region Required validations
            SharedValidationMethods.ValidateStringRequired(contract.FirstName, nameof(contract.FirstName), ref messages);
            SharedValidationMethods.ValidateStringRequired(contract.LastName, nameof(contract.LastName), ref messages);

            if (contract.Status == CustomerStatus.Unknown)
            {
                messages.Add(new ErrorMessage(
                    nameof(contract.Status),
                    $"{nameof(contract.Status)} is not recognized. Allowed values are {CustomerStatus.Prospective.Value}, {CustomerStatus.Current.Value}, {CustomerStatus.NonActive.Value}")
                );
            }
            #endregion

            #region Length validations
            SharedValidationMethods.ValidateStringLength(contract.FirstName, 30, nameof(contract.FirstName), ref messages);
            SharedValidationMethods.ValidateStringLength(contract.LastName, 70, nameof(contract.LastName), ref messages);
            #endregion

            return Task.FromResult(messages);
        }

        public bool AbortOnFailure() => false;
        public Type GetValidationContractType() => typeof(ICustomerInformationValidationContract);
    }
}
