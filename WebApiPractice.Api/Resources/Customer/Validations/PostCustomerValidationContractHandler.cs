using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Api.ValidationFlow;
using WebApiPractice.Api.ValidationFlow.Interfaces;
using WebApiPractice.Persistent.Context;

namespace WebApiPractice.Api.Resources.Customer.Validations
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
        private readonly AppDbContext _appDbContext;
        public PostCustomerValidationContractHandler(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }
        #endregion

        public async Task<List<ErrorMessage>> Handle(IValidationContract request, CancellationToken cancellationToken = default)
        {
            if (!(request is IPostCustomerValidationContract contract))
            {
                var errorMessage = $"Validation Handler {nameof(PostCustomerValidationContractHandler)}" +
                                   $" could not find contract: {nameof(IPostCustomerValidationContract)}";
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
                    SharedValidationMethods.ValidaStringLength(contract.ContactDetails[i].Details,  100, $"ContactDetails[{i + 1}].Details", ref messages);
                }
            }
            #endregion

            #region Length validations
            SharedValidationMethods.ValidaStringLength(contract.FirstName, 30, nameof(contract.FirstName), ref messages);
            SharedValidationMethods.ValidaStringLength(contract.LastName, 70, nameof(contract.LastName), ref messages);
            #endregion

            return messages;
        }

        public bool AbortOnFailure() => false;
        public Type GetValidationContractType() => typeof(IPostCustomerValidationContract);
    }
}
