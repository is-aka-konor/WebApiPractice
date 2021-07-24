using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Api.ValidationFlow;
using WebApiPractice.Api.ValidationFlow.Interfaces;

namespace WebApiPractice.Api.Resources.Customers.Validations
{
    /// <summary>
    /// Describes the contract to validate a request of type <see cref="PostCustomerRequest"/>
    /// </summary>
    public interface IPostCustomerValidationContract : IValidationContract
    {
        public CustomerStatus Status { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<PostContactDetailsRequest> ContactDetails { get; set; }
    }

    /// <summary>
    /// This class does the actual validation of the request of type <see cref="PostCustomerRequest"/>
    /// </summary>
    public class PostCustomerValidationContractHandler : IValidationContractHandler
    {
        #region Constructor & private fields
        public PostCustomerValidationContractHandler() { }
        #endregion

        public Task<List<ErrorMessage>> Handle(IValidationContract request, CancellationToken cancellationToken = default)
        {
            if (!(request is IPostCustomerValidationContract contract))
            {
                var errorMessage = $"Validation Handler {nameof(PostCustomerValidationContractHandler)}" +
                                   $" could not find contract: {nameof(IPostCustomerValidationContract)}";
                throw new Exception(errorMessage);
            }
            var messages = new List<ErrorMessage>();

            #region Required validations
            if(contract.ContactDetails is not null)
            {
                for (var i = 0; i < contract.ContactDetails.Count; ++i)
                {
                    if (contract.ContactDetails[i].ContactDetailsType == ContactDetailsType.Unknown)
                    {
                        messages.Add(new ErrorMessage(
                            $"ContactDetails[{i + 1}].ContactDetailsType",
                            $"ContactDetails[{i + 1}].ContactDetailsType is not recognized. Allowed values are {ContactDetailsType.Email.Value}, {ContactDetailsType.Phone.Value}, {ContactDetailsType.Website.Value}")
                        );
                    }
                    SharedValidationMethods.ValidateStringRequired(contract.ContactDetails[i].Details, $"ContactDetails[{i + 1}].Details", ref messages);
                    SharedValidationMethods.ValidateStringLength(contract.ContactDetails[i].Details,  100, $"ContactDetails[{i + 1}].Details", ref messages);
                }
            }
            #endregion
            return Task.FromResult(messages);
        }

        public bool AbortOnFailure() => false;
        public Type GetValidationContractType() => typeof(IPostCustomerValidationContract);
    }
}
