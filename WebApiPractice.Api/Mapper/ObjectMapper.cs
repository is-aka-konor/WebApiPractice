using System;
using System.Collections.Generic;
using System.Linq;
using WebApiPractice.Api.Resources.Customers;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Api.Mapper
{
    /// <summary>
    /// Mapper between database objects and request/response objects
    /// </summary>
    public class ObjectMapper : IObjectMapper
    {
        #region Generic methods
        public TDest Map<TSource, TDest>(TSource item)
        {
            object result = item switch
            {
                PostCustomerRequest postCustomerRequest => Map(postCustomerRequest),
                Customer customer => Map(customer),
                PostContactDetailsRequest postContactDetailsRequest => Map(postContactDetailsRequest),
                ContactDetails contactDetails => Map(contactDetails),
                _ => throw new NotSupportedException()
            };

            return (TDest)result;
        }

        public IEnumerable<TDest> Map<TSource, TDest>(IEnumerable<TSource> sourceCollection) => sourceCollection.Select(Map<TSource, TDest>);
        #endregion

        #region Private methods
        private Customer Map(PostCustomerRequest request)
        {
            return new Customer()
            {
                CustomerExternalId = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                Status = request.Status.Value,
                FirstName = request.FirstName,
                LastName = request.LastName,
                ContactDetails = Map<PostContactDetailsRequest, ContactDetails>(request.ContactDetails).ToList()
            };
        }

        private UpdateCustomerReponse Map(Customer customer)
        {
            return new UpdateCustomerReponse()
            {
                Status = customer.Status,
                CreatedAt = customer.CreatedAt,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                ContactDetails = Map<ContactDetails, PostContactDetailsResponse>(customer.ContactDetails).ToList(),
                CustomerExternalId = customer.CustomerExternalId
            };
        }

        //private GetCustomerResponse Map(UpdateCustomerReponse customer)
        //{
        //    return customer as GetCustomerResponse;
        //}

        private ContactDetails Map(PostContactDetailsRequest request)
        {
            return new ContactDetails()
            {
                ContactDetailsType = request.ContactDetailsType.Value,
                Details = request.Details
            };
        }

        private PostContactDetailsResponse Map(ContactDetails contactDetails)
        {
            return new PostContactDetailsResponse()
            {
                ContactDetailsType = contactDetails.ContactDetailsType,
                Details = contactDetails.Details
            };
        }
        #endregion
    }
}
