using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.ValidationFlow;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Api.ValidationFlow.Interfaces;

namespace WebApiPractice.Api.Resources.Customer.Validations
{
    public interface IGetCustomersValidationContract : IValidationContract
    {
        public string NextCursor { get; set; }
        public string Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class GetCustomersValidationContractHandler : IValidationContractHandler
    {
        public Task<List<ErrorMessage>> Handle(IValidationContract request, CancellationToken cancellationToken = default)
        {
            if (!(request is IGetCustomersValidationContract contract))
            {
                var errorMessage = $"Validation Handler {nameof(GetCustomersValidationContractHandler)}" +
                                   $" could not find contract: {nameof(IGetCustomersValidationContract)}";
                throw new Exception(errorMessage);
            }
            var messages = new List<ErrorMessage>();

            if (!string.IsNullOrEmpty(contract.Status))
            {
                try
                {
                    var enumStatus = Enumeration.FromValue<CustomerStatus>(contract.Status);
                }
                catch (Exception)
                {
                    messages.Add(new ErrorMessage(
                        nameof(contract.Status),
                        $"{nameof(contract.Status)} is not recognized. Allowed values are {CustomerStatus.Prospective.Value}, {CustomerStatus.Current.Value}, {CustomerStatus.NonActive.Value}")
                    );
                }                
            }

            #region Length validations
            SharedValidationMethods.ValidateStringLength(contract.FirstName, 30, nameof(contract.FirstName), ref messages);
            SharedValidationMethods.ValidateStringLength(contract.LastName, 70, nameof(contract.LastName), ref messages);
            #endregion

            return Task.FromResult(messages);
        }

        public bool AbortOnFailure() => true;
        public Type GetValidationContractType() => typeof(IGetCustomersValidationContract);
    }
}
