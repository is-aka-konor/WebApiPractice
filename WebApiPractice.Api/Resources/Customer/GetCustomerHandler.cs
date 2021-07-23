using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Mapper;
using System.Collections.Generic;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Api.Resources.Customer.Validations;

using DbCustomer = WebApiPractice.Persistent.DataModels.Customer;
using Microsoft.EntityFrameworkCore;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.ResponseStructure;
using Microsoft.Extensions.Logging;

namespace WebApiPractice.Api.Resources.Customer
{
    /// <summary>
    /// Describes a model of incoming request to get a customer
    /// </summary>
    public class GetCustomerRequest : IRequest<GetCustomerResponse>
        , ICustomerNotFoundValidationContract
    {
        public string CustomerExternalId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Describes a model of the response to <see cref="GetCustomerRequest"/>
    /// </summary>
    public class GetCustomerResponse
    {
        public Guid CustomerExternalId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<PostContactDetailsResponse> ContactDetails { get; set; } = null!;
    }

    /// <summary>
    /// Handles get customer by an external id request
    /// </summary>
    public class GetCustomerHandler : IRequestHandler<GetCustomerRequest, GetCustomerResponse>
    {
        #region Private fields and constructor
        private readonly AppDbContext _appDbContext;
        private readonly IObjectMapper _mapper;
        private readonly ILogger<GetCustomerHandler> _logger;
        public GetCustomerHandler(AppDbContext appDbContext,
            IObjectMapper mapper,
            ILogger<GetCustomerHandler> logger)
        {
            this._appDbContext = appDbContext;
            this._logger = logger;
            this._mapper = mapper;
        }
        #endregion
        public async Task<GetCustomerResponse> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
        {
            var externalId = Guid.TryParse(request.CustomerExternalId, out var guid) ? guid : Guid.Empty;
            if(externalId == Guid.Empty)
            {
                // If validation contracts were applied correctly then we should not be here
                this._logger.LogWarning($"A get customer request with unrecognized Guid {request.CustomerExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {request.CustomerExternalId}");
            }
            var customer = await this._appDbContext.Customers.Include(c => c.ContactDetails).FirstOrDefaultAsync(x => x.CustomerExternalId.Equals(externalId));
            return this._mapper.Map<DbCustomer, GetCustomerResponse>(customer);
        }
    }
}
