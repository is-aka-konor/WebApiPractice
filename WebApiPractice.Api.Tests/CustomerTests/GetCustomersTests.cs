using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.Extensions;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customers;
using WebApiPractice.Api.Resources.Customers.Validations;
using WebApiPractice.Api.Tests.TestBuilder;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Api.Tests.CustomerTests
{
    [TestClass]
    public class GetCustomersTests
    {
        protected Mock<ILogger<GetCustomersHandler>> LoggerMock = LoggerHelper.GetLogger<GetCustomersHandler>();
        private GetCustomersHandler _getCustomersHandler;
        private GetCustomersValidationContractHandler _getCustomersValidationContractHandler = new GetCustomersValidationContractHandler();
        private Guid _currentExistingCustomerExternalId;
        private Guid _nonActiveExistingCustomerExternalId;
        private Guid _prospectiveExistingCustomerExternalId;
        private AppDbContext _appDbContex;

        [TestInitialize]
        public void Initialize()
        {
            this._currentExistingCustomerExternalId = Guid.NewGuid();
            this._nonActiveExistingCustomerExternalId = Guid.NewGuid();
            this._prospectiveExistingCustomerExternalId = Guid.NewGuid();

            var customers = new List<Customer>()
            {
                new Customer()
                {
                    FirstName = "Unit",
                    LastName = "Test",
                    Status = CustomerStatus.Current.Value,
                    CreatedAt = DateTime.Now.AddDays(-3),
                    CustomerExternalId = this._currentExistingCustomerExternalId,
                    CustomerId = 1,
                    ContactDetails = new List<ContactDetails>()
                },
                new Customer()
                {
                    FirstName = "Unit 1",
                    LastName = "Test 1",
                    Status = CustomerStatus.NonActive.Value,
                    CreatedAt = DateTime.Now.AddDays(-2),
                    CustomerExternalId = this._nonActiveExistingCustomerExternalId,
                    CustomerId = 2,
                    ContactDetails = new List<ContactDetails>()
                },
                new Customer()
                {
                    FirstName = "Unit 2",
                    LastName = "Test 2",
                    Status = CustomerStatus.NonActive.Value,
                    CreatedAt = DateTime.Now.AddDays(-1),
                    CustomerExternalId = this._prospectiveExistingCustomerExternalId,
                    CustomerId = 3,
                    ContactDetails = new List<ContactDetails>()
                },
                new Customer()
                {
                    FirstName = "Unit 3",
                    LastName = "Test 3",
                    Status = CustomerStatus.Prospective.Value,
                    CreatedAt = DateTime.Now,
                    CustomerExternalId = Guid.NewGuid(),
                    CustomerId = 4,
                    ContactDetails = new List<ContactDetails>()
                }
            };
            var mapper = new ObjectMapper();
            var dbContext = new AppDbContextBuilder()
                .UseInMemorySqlite()
                .WithCustomers(customers)
                .Build();
            this._appDbContex = dbContext;
            this._getCustomersHandler = new GetCustomersHandler(this._appDbContex, mapper, LoggerMock.Object);
        }

        [TestMethod, Description("Validate an invalid Status field")]
        public async Task Should_FailValidation_WhenStatusIsNotRecognised()
        {
            // Arrange 
            var request = new GetCustomersRequest()
            {
                Status = "123"
            };
            // Act
            var result = await this._getCustomersValidationContractHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("Status is not recognized")), "Status filter value should be from the enumeration list");
        }

        [TestMethod, Description("Validate too long filter fields")]
        public async Task Should_FailValidation_WhenFiltersAreTooLong()
        {
            // Arrange 
            var request = new GetCustomersRequest()
            {
                FirstName = new string('a', 31),
                LastName = new string('a', 71),
                Status = CustomerStatus.Current.Value
            };
            // Act
            var result = await this._getCustomersValidationContractHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("FirstName must not exceed 30")), "FirstName cannot be longer than 30 characters");
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("LastName must not exceed 70")), "LastName cannot be longer than 70 characters");
        }

        [TestMethod, Description("Validate limit filter")]
        public async Task Should_RespectLimitParameter()
        {
            // Arrange 
            var request = new GetCustomersRequest()
            {
                Limit = 1
            };
            // Act
            var result = await this._getCustomersHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Customers.Count == 1);
            Assert.IsTrue(result.ResponseMetadata.HasNext);
        }

        [TestMethod, Description("Validate status filter")]
        public async Task Should_FilterByStatus()
        {
            // Arrange 
            var request = new GetCustomersRequest()
            {
                Limit = 50,
                Status = CustomerStatus.NonActive.Value
            };
            // Act
            var result = await this._getCustomersHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Customers.Count == 2);
        }

        [TestMethod, Description("Validate first name")]
        public async Task Should_FilterByFirstName()
        {
            // Arrange 
            var request = new GetCustomersRequest()
            {
                Limit = 50,
                FirstName = "Unit 1"
            };
            // Act
            var result = await this._getCustomersHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Customers.Count == 1);
        }

        [TestMethod, Description("Validate last name")]
        public async Task Should_FilterByLastName()
        {
            // Arrange 
            var request = new GetCustomersRequest()
            {
                Limit = 50,
                LastName = "Test 3"
            };
            // Act
            var result = await this._getCustomersHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Customers.Count == 1);
        }

        [TestMethod, Description("Validate complex filter")]
        public async Task Should_FilterByComplextFilter()
        {
            // Arrange 
            var request = new GetCustomersRequest()
            {
                Limit = 50,
                LastName = "Test 1",
                Status = CustomerStatus.NonActive.Value
            };
            // Act
            var result = await this._getCustomersHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Customers.Count == 1);
        }

        [TestMethod, Description("Validate the next cursor")]
        public async Task Should_UseCursorForPagination()
        {
            // Arrange 
            var request = new GetCustomersRequest()
            {
                Limit = 1,
                NextCursor = 3.Base64Encode()
            };
            // Act
            var result = await this._getCustomersHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Customers.Count == 1);
            Assert.IsTrue(result.Customers[0].CustomerExternalId == this._prospectiveExistingCustomerExternalId);
        }
    }
}
