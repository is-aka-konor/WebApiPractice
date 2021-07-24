using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Mapper;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Api.Resources.Customers.Validations;

using DbCustomer = WebApiPractice.Persistent.DataModels.Customer;
using Newtonsoft.Json;

namespace WebApiPractice.Api.Resources.Customers
{
    /// <summary>
    /// Describes a model of incoming request to get a customer
    /// </summary>
    public class GetCustomerRequest : IRequest<GetCustomerResponse>
        , ICustomerNotFoundValidationContract
    {
        public string ExternalId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Describes a model of the response to <see cref="GetCustomerRequest"/>
    /// </summary>
    public class GetCustomerResponse
    {
        [JsonIgnore]
        public string RowVersion { get; set; } = string.Empty;
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
            _appDbContext = appDbContext;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion
        public async Task<GetCustomerResponse> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
        {
            var externalId = Guid.TryParse(request.ExternalId, out var guid) ? guid : Guid.Empty;
            if (externalId == Guid.Empty)
            {
                // If validation contracts were applied correctly then we should not be here
                _logger.LogWarning($"A get customer request with unrecognized Guid {request.ExternalId} by pass validation. Please investigate.");
                throw new ResourceNotFoundException($"{ErrorCode.ResourceNotFound.Message} Resource Id: {request.ExternalId}");
            }
            var customer = await _appDbContext.Customers.Include(c => c.ContactDetails).FirstOrDefaultAsync(x => x.CustomerExternalId.Equals(externalId));
            return _mapper.Map<DbCustomer, GetCustomerResponse>(customer);
        }
    }
}
