using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customer;
using WebApiPractice.Api.Resources.Customer.Validations;
using WebApiPractice.Api.Tests.TestBuilder;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Api.Tests.CustomerTests
{
    [TestClass]
    public class PostCustomerTests
    {
        private PostCustomerValidationContractHandler _postCustomerValidationHandler = new PostCustomerValidationContractHandler();
        private PostCustomerHandler _postCustomerHandler;
        private AppDbContext _appDbContex;
        [TestInitialize]
        public void Initialize()
        {
            var customers = new List<Customer>();
            var mapper = new ObjectMapper();
            var dbContext = new AppDbContextBuilder()
                .UseInMemorySqlite()
                .WithCustomers(customers)
                .Build();
            this._appDbContex = dbContext;
            _postCustomerHandler = new PostCustomerHandler(this._appDbContex, mapper);
        }

        [TestMethod, Description("Validate required fields")]
        public async Task Should_FailValidation_WhenCustomerDetailsInvalid()
        {
            // Arrange 
            var request = new PostCustomerRequest()
            {
                ContactDetails = new List<PostContactDetailsRequest>()
                {
                    new PostContactDetailsRequest()
                    {
                        ContactDetailsType = ContactDetailsType.Unknown,
                        Details = string.Empty
                    }
                }
            };
            // Act
            var result = await this._postCustomerValidationHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("ContactDetails[1].ContactDetailsType is not recognized")), "ContactDetailsType cannot be Unknown");
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("ContactDetails[1].Details is required")), "Contact Details cannot be empty or null");
        }

        [TestMethod, Description("Validate details field length")]
        public async Task Should_FailValidation_WhenCustomerDetailsIsTooLong()
        {
            // Arrange 
            var request = new PostCustomerRequest()
            {
                ContactDetails = new List<PostContactDetailsRequest>()
                {
                    new PostContactDetailsRequest()
                    {
                        ContactDetailsType = ContactDetailsType.Phone,
                        Details = new string('d', 101),
                    }
                }
            };
            // Act
            var result = await this._postCustomerValidationHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("ContactDetails[1].Details must not exceed 100")), "Contact Details cannot be longer than 100 characters");
        }

        [TestMethod, Description("Request is valid")]
        public async Task Should_PassValidation_WhenAllRequiredFieldsAreProvided()
        {
            // Arrange 
            var request = new PostCustomerRequest()
            {
                FirstName = "First name",
                LastName = "Last name",
                Status = CustomerStatus.Current,
                ContactDetails = new List<PostContactDetailsRequest>()
                {
                    new PostContactDetailsRequest()
                    {
                        ContactDetailsType = ContactDetailsType.Phone,
                        Details = "+64222223333111"
                    }
                }
            };
            // Act
            var result = await this._postCustomerValidationHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 0, "should pass all validations with valid request");
        }

        [TestMethod, Description("Request is valid")]
        public async Task Should_SaveCustomer()
        {
            // Arrange 
            var request = new PostCustomerRequest()
            {
                FirstName = "First name",
                LastName = "Last name",
                Status = CustomerStatus.Current,
                ContactDetails = new List<PostContactDetailsRequest>()
                {
                    new PostContactDetailsRequest()
                    {
                        ContactDetailsType = ContactDetailsType.Phone,
                        Details = "+64222223333111"
                    }
                }
            };
            // Act
            var result = await this._postCustomerHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(this._appDbContex.Customers.SingleOrDefault(c => c.LastName.Equals(request.LastName)), "should information from a valid request");
        }
    }
}
