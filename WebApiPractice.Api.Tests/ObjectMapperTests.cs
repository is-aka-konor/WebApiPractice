using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customers;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Api.Tests
{
    [TestClass]
    public class ObjectMapperTests
    {
        private readonly IObjectMapper _mapper = new ObjectMapper();
        
        [TestMethod]
        public void Should_ThrowNotSupportException()
        {
            Assert.ThrowsException<NotSupportedException>(() => this._mapper.Map<int, string>(2));
        }

        [TestMethod]
        public void Should_ThrowNotSupportException_ForNulls()
        {
            Assert.ThrowsException<NotSupportedException>(() => this._mapper.Map<PostCustomerRequest, Customer>((PostCustomerRequest)null));
        }

        [TestMethod]
        public void Should_ThrowNotSupportException_ForIEnumerableNulls()
        {
            Assert.ThrowsException<ArgumentNullException>(() => this._mapper.Map<PostContactDetailsRequest, ContactDetails>((List<PostContactDetailsRequest>)null));
        }

        [TestMethod]
        public void Should_CorrectlyMap_PostCustomerReqestToCustomer()
        {
            // Arrange
            var contactDetails = new List<PostContactDetailsRequest>()
            {
                new PostContactDetailsRequest()
                {
                    ContactDetailsType = ContactDetailsType.Phone,
                    Details = "+64 222 050 2638"
                }
            };
            var source = new PostCustomerRequest()
            {
                ContactDetails = contactDetails,
                FirstName = "First",
                LastName = "Family",
                Status = CustomerStatus.Prospective
            };
            // Act
            var target = this._mapper.Map<PostCustomerRequest, Customer>(source);
            // Assert
            Assert.AreEqual(source.Status.Value, target.Status);
            Assert.AreEqual(source.FirstName, target.FirstName);
            Assert.AreEqual(source.LastName, target.LastName);
            Assert.IsNotNull(target.ContactDetails);
            Assert.AreEqual(source.ContactDetails[0].ContactDetailsType.Value, target.ContactDetails[0].ContactDetailsType);
            Assert.AreEqual(source.ContactDetails[0].Details, target.ContactDetails[0].Details);
        }
    }
}
