using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Mapper;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Api.ResponseStructure;
using WebApiPractice.Api.Resources.Customers.Validations;

using DbCustomer = WebApiPractice.Persistent.DataModels.Customer;
using WebApiPractice.Api.Extensions;
using WebApiPractice.Api.Enumerations;
using System.Linq;
using WebApiPractice.Api.Resources.Customers;

namespace WebApiPractice.Api.Resources.Customers
{
    /// <summary>
    /// Describes a model of incoming request to get customers
    /// </summary>
    public class GetCustomersRequest : IRequest<GetCustomersResponse>
        , IGetCustomersValidationContract
    {
        public string NextCursor { get; set; } = string.Empty;
        public int Limit { get; set; }
        public string Status { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Describes a model of the response to <see cref="GetCustomersRequest"/>
    /// </summary>
    public class GetCustomersResponse
    {
        public List<GetCustomerResponse> Customers { get; set; }
        public ResponseMetadata ResponseMetadata { get; set; }

        public GetCustomersResponse()
        {
            Customers = new List<GetCustomerResponse>();
            ResponseMetadata = new ResponseMetadata();
        }
    }

    /// <summary>
    /// Handles get customers request
    /// </summary>
    public class GetCustomersHandler : IRequestHandler<GetCustomersRequest, GetCustomersResponse>
    {
        #region Private fields and constructor
        private readonly AppDbContext _appDbContext;
        private readonly IObjectMapper _mapper;
        private readonly ILogger<GetCustomersHandler> _logger;
        public GetCustomersHandler(AppDbContext appDbContext,
            IObjectMapper mapper,
            ILogger<GetCustomersHandler> logger)
        {
            this._appDbContext = appDbContext;
            this._logger = logger;
            this._mapper = mapper;
        }
        #endregion
        public async Task<GetCustomersResponse> Handle(GetCustomersRequest request, CancellationToken cancellationToken)
        {
            var response = new GetCustomersResponse();
            var cursor = request.NextCursor.Base64DecodeInt();
            var query = this._appDbContext.Customers.AsNoTracking()
                .Include(c => c.ContactDetails)
                .Where(c => c.CustomerId >= cursor);

            if (!string.IsNullOrEmpty(request.Status))
            {
                try
                {
                    var customerStatus = Enumeration.FromValue<CustomerStatus>(request.Status);
                    if (customerStatus != CustomerStatus.Unknown)
                    {
                        query = query.Where(c => c.Status.Equals(customerStatus.Value));
                    }
                }
                catch (Exception)
                {
                    this._logger.LogWarning($"A get customers request with unrecognized status {request.Status} by pass validation. Please investigate.");
                }
            }            

            if(!string.IsNullOrEmpty(request.FirstName))
            {
                query = query.Where(c => c.FirstName.Contains(request.FirstName));
            }

            if (!string.IsNullOrEmpty(request.LastName))
            {
                query = query.Where(c => c.LastName.Contains(request.LastName));
            }

            var customers = await query.OrderBy(c => c.CreatedAt)
                                        .Select(c => c)
                                        .ToListAsync(request.Limit,
                                        (customer) =>
                                        {
                                            response.ResponseMetadata.HasNext = true;
                                            response.ResponseMetadata.NextCursor = customer.CustomerId.Base64Encode();
                                        }, cancellationToken);
            response.Customers = this._mapper.Map<DbCustomer, GetCustomerResponse>(customers).ToList();
            return response;
        }
    }
}
