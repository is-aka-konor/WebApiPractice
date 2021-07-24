using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customers.Validations;
using WebApiPractice.Api.Resources.Customers;
using WebApiPractice.Api.Tests.TestBuilder;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Api.Tests.CustomerTests
{
    [TestClass]
    public class GetCustomerTests
    {
        protected Mock<ILogger<GetCustomerHandler>> LoggerMock = LoggerHelper.GetLogger<GetCustomerHandler>();
        private GetCustomerHandler _getCustomerHandler;
        private CustomerNotFoundValidationContractHandler _customerNotFoundValidationContractHandler;
        private Guid _existingCustomerExternalId;
        private AppDbContext _appDbContext;
        
        [TestInitialize]
        public void Initialize()
        {
            this._existingCustomerExternalId = Guid.NewGuid();

            var customers = new List<Customer>()
            {
                new Customer()
                {
                    FirstName = "Unit",
                    LastName = "Test",
                    Status = CustomerStatus.Current.Value,
                    CreatedAt = DateTime.Now.AddDays(-1),
                    CustomerExternalId = this._existingCustomerExternalId,
                    CustomerId = 1,
                    ContactDetails = new List<ContactDetails>()
                }
            };
            var mapper = new ObjectMapper();
            var dbContext = new AppDbContextBuilder()
                .UseInMemorySqlite()
                .WithCustomers(customers)
                .Build();
            this._appDbContext = dbContext;
            this._getCustomerHandler = new GetCustomerHandler(this._appDbContext, mapper, LoggerMock.Object);
            this._customerNotFoundValidationContractHandler = new CustomerNotFoundValidationContractHandler(this._appDbContext);
        }

        [TestMethod, Description("Validate valid path")]
        public async Task Should_ReturnCustomer_WhenCustomerExternalIdExists()
        {
            // Arrange 
            var request = new GetCustomerRequest()
            {
                ExternalId = this._existingCustomerExternalId.ToString()
            };
            // Act
            var result = await this._getCustomerHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(this._existingCustomerExternalId, result.CustomerExternalId);
        }

        [TestMethod, Description("Validate invalid Guid field")]
        public async Task Should_ThrowResourceNotFoundException_WhenCustomerExternalIdIsNotGuid()
        {
            // Arrange 
            var request = new GetCustomerRequest()
            {
                ExternalId = "123"
            };
            // Act
            var result = this._getCustomerHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            var ex = await Assert.ThrowsExceptionAsync<ResourceNotFoundException>(() => result);
        }

        [TestMethod, Description("Validate invalid Guid field")]
        public async Task Should_FailValidation_WhenCustomerExternalIdIsNotGuid()
        {
            // Arrange 
            var request = new GetCustomerRequest()
            {
                ExternalId = "123"
            };
            // Act
            var result = await this._customerNotFoundValidationContractHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("CustomerId must be a valid Guid.")), "CustomerId cannot be not Guid");
        }

        [TestMethod, Description("Validate valid Guid field not found")]
        public async Task Should_ThrowResourceNotFoundException__WhenCustomerExternalIdNotFound()
        {
            // Arrange 
            var request = new GetCustomerRequest()
            {
                ExternalId = Guid.NewGuid().ToString()
            };
            // Act
            var result = this._customerNotFoundValidationContractHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            var ex = await Assert.ThrowsExceptionAsync<ResourceNotFoundException>(() => result);
        }
    }
}
