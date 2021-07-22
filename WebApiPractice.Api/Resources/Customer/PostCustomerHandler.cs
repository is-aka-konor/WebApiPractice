﻿using MediatR;
using System;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Mapper;
using System.Collections.Generic;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Api.Enumerations;

using DbCustomer = WebApiPractice.Persistent.DataModels.Customer;

namespace WebApiPractice.Api.Resources.Customer
{
    /// <summary>
    /// Handles the creation of a new customer
    /// </summary>
    public class PostCustomerHandler : IRequestHandler<PostCustomerRequest, PostCustomerResponse>
    {
        #region Private fields and constructor
        private readonly AppDbContext _appDbContext;
        private readonly IObjectMapper _mapper;
        public PostCustomerHandler(AppDbContext appDbContext,
            IObjectMapper mapper)
        {
            this._appDbContext = appDbContext;
            this._mapper = mapper;
        }
        #endregion

        public async Task<PostCustomerResponse> Handle(PostCustomerRequest request, CancellationToken cancellationToken)
        {
            var customer = this._mapper.Map<PostCustomerRequest, DbCustomer>(request);
            await this._appDbContext.Customers.AddAsync(customer, cancellationToken).ConfigureAwait(false);
            await this._appDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return this._mapper.Map<DbCustomer, PostCustomerResponse>(customer);
        }
    }

    /// <summary>
    /// Describes a model of incoming request to create a customer
    /// </summary>
    public class PostCustomerRequest : IRequest<PostCustomerResponse>
    {
        [JsonConverter(typeof(EnumerationConverter<CustomerStatus>))]
        public CustomerStatus Status { get; set; } = CustomerStatus.Unknown;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<PostContactDetailsRequest> ContactDetails { get; set; } = null!;
    }

    /// <summary>
    /// Describes a model of the response to <see cref="PostCustomerRequest"/>
    /// </summary>
    public class PostCustomerResponse
    {
        public Guid CustomerExternalId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<PostContactDetailsResponse> ContactDetails { get; set; } = null!;
    }
}
