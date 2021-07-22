using MediatR;
using System;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Persistent.Context;

namespace WebApiPractice.Api.Resources.Customer
{
    /// <summary>
    /// Handles the creation of a new customer details, might be implemented in the future if required
    /// </summary>
    public class PostContactDetailsHandler : IRequestHandler<PostContactDetailsRequest, PostContactDetailsResponse>
    {
        #region Private fields and constructor
        private readonly AppDbContext _appDbContext;
        public PostContactDetailsHandler(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }
        #endregion

        public Task<PostContactDetailsResponse> Handle(PostContactDetailsRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Describes a model of incoming request to create a customer contact details
    /// </summary>
    public class PostContactDetailsRequest : IRequest<PostContactDetailsResponse>
    {
        [JsonConverter(typeof(EnumerationConverter<ContactDetailsType>))]
        public ContactDetailsType ContactDetailsType { get; set; } = ContactDetailsType.Unknown;
        public string Details { get; set; } = string.Empty;
    }

    /// <summary>
    /// Describes a model of the response to <see cref="PostContactDetailsRequest"/>
    /// </summary>
    public class PostContactDetailsResponse
    {
        public int ContactDetailsId { get; set; }
        public string ContactDetailsType { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}