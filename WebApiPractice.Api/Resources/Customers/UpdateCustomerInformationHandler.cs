using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customers.Validations;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Persistent.Context;

using DbCustomer = WebApiPractice.Persistent.DataModels.Customer;

namespace WebApiPractice.Api.Resources.Customers
{
    /// <summary>
    /// Describes a model of incoming request to update the customer
    /// </summary>
    public class UpdateCustomerRequest : IRequest<UpdateCustomerReponse>
        , ICustomerNotFoundValidationContract
        , ICustomerInformationValidationContract
    {
        [JsonIgnore]
        public string CustomerExternalId { get; set; } = string.Empty;
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
        private readonly AppDbContext _appDbContext;
        private readonly IObjectMapper _mapper;
        private readonly ILogger<UpdateCustomerInformationHandler> _logger;
        public UpdateCustomerInformationHandler(AppDbContext appDbContext,
            IObjectMapper mapper,
            ILogger<UpdateCustomerInformationHandler> logger)
        {
            this._appDbContext = appDbContext;
            this._logger = logger;
            this._mapper = mapper;
        }
        #endregion
        public async Task<UpdateCustomerReponse> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
        {
            var externalId = Guid.TryParse(request.CustomerExternalId, out var guid) ? guid : Guid.Empty;
            if (externalId == Guid.Empty)
            {
                // If validation contracts were applied correctly then we should not be here
                this._logger.LogWarning($"An update customer request with unrecognized Guid {request.CustomerExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {request.CustomerExternalId}");
            }
            var customer = await this._appDbContext.Customers.Include(c => c.ContactDetails).FirstOrDefaultAsync(x => x.CustomerExternalId.Equals(externalId));
            if (customer is null)
            {
                // If validation contracts were applied correctly then we should not be here
                this._logger.LogWarning($"An update customer request with customer external id: {request.CustomerExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {request.CustomerExternalId}");
            }
            customer.Status = request.Status.Value; 
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;            
            this._appDbContext.Customers.Update(customer);
            await this._appDbContext.SaveChangesAsync();
            return this._mapper.Map<DbCustomer, UpdateCustomerReponse>(customer);
        }
    }
}
