using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customers.Validations;
using WebApiPractice.Api.Resources.SharedValidations;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Persistent.Repositories;
using DbCustomer = WebApiPractice.Persistent.DataModels.Customer;

namespace WebApiPractice.Api.Resources.Customers
{
    /// <summary>
    /// Describes a model of incoming request to update the customer
    /// </summary>
    public class UpdateCustomerRequest : IRequest<UpdateCustomerReponse>
        , ICustomerNotFoundValidationContract
        , IRowVersionMatchValidationContractHandler
        , ICustomerInformationValidationContract
    {
        [JsonIgnore]
        public string ExternalId { get; set; } = string.Empty;
        [JsonIgnore]
        public string RowVersion { get; set; } = string.Empty;
        [JsonConverter(typeof(EnumerationConverter<CustomerStatus>))]
        public CustomerStatus Status { get; set; } = CustomerStatus.Unknown;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Describes a model of the response to <see cref="UpdateCustomerRequest"/>
    /// </summary>
    public class UpdateCustomerReponse : PostCustomerResponse
    {
    } 

    /// <summary>
    /// Handles update customer requests of type <see cref="UpdateCustomerRequest"/>
    /// </summary>
    public class UpdateCustomerInformationHandler : IRequestHandler<UpdateCustomerRequest, UpdateCustomerReponse>
    {
        #region Private fields and constructor
        private readonly ICustomerRepository _repository;
        private readonly IObjectMapper _mapper;
        private readonly ILogger<UpdateCustomerInformationHandler> _logger;
        public UpdateCustomerInformationHandler(ICustomerRepository repository,
            IObjectMapper mapper,
            ILogger<UpdateCustomerInformationHandler> logger)
        {
            this._repository = repository;
            this._logger = logger;
            this._mapper = mapper;
        }
        #endregion
        public async Task<UpdateCustomerReponse> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
        {
            var externalId = Guid.TryParse(request.ExternalId, out var guid) ? guid : Guid.Empty;
            if (externalId == Guid.Empty)
            {
                // If validation contracts were applied correctly then we should not be here
                this._logger.LogWarning($"An update customer request with unrecognized Guid {request.ExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {request.ExternalId}");
            }
            var customer = await this._repository.GetCustomerByExternalId(externalId);
            if (customer is null)
            {
                // If validation contracts were applied correctly then we should not be here
                this._logger.LogWarning($"An update customer request with customer external id: {request.ExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {request.ExternalId}");
            }
            customer.Status = request.Status.Value; 
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;            
            customer = await this._repository.UpdateCustomer(customer);
            return this._mapper.Map<DbCustomer, UpdateCustomerReponse>(customer);
        }
    }
}
