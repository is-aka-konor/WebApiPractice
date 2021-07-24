using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.Exceptions;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customers;
using WebApiPractice.Api.Resources.SharedValidations;
using WebApiPractice.Api.Tests.TestBuilder;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Persistent.DataModels;
using WebApiPractice.Persistent.Repositories;

namespace WebApiPractice.Api.Tests.CustomerTests
{
    [TestClass]
    public class UpdateCustomerTests
    {
        protected Mock<ILogger<UpdateCustomerInformationHandler>> LoggerMock = LoggerHelper.GetLogger<UpdateCustomerInformationHandler>();
        private RowVersionMatchValidationContractHandler _rowVersionMatchValidationContractHandler;
        private UpdateCustomerInformationHandler _updateCustomerInformationHandler;
        private Guid _currentExistingCustomerExternalId;
        private AppDbContext _appDbContext;

        [TestInitialize]
        public void Initialize()
        {
            this._currentExistingCustomerExternalId = Guid.NewGuid();

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
                    ContactDetails = new List<ContactDetails>(),
                    RowVersion = RowVersionGenerator.GetVersion()
                }
            };
            var mapper = new ObjectMapper();
            var dbContext = new AppDbContextBuilder()
                .UseInMemorySqlite()
                .WithCustomers(customers)
                .Build();
            this._appDbContext = dbContext;
            this._updateCustomerInformationHandler = new UpdateCustomerInformationHandler(new CustomerRepository(this._appDbContext), mapper, LoggerMock.Object);
            this._rowVersionMatchValidationContractHandler = new RowVersionMatchValidationContractHandler(
                                                                    new CustomerRepository(this._appDbContext),
                                                                    LoggerHelper.GetLogger<RowVersionMatchValidationContractHandler>().Object);
        }

        [TestMethod, Description("Customer information updated")]
        public async Task Should_UpdateCustomerInformation_WhenValidRequest()
        {
            // Arrange 
            var request = new UpdateCustomerRequest()
            {
                CustomerExternalId = this._currentExistingCustomerExternalId.ToString(),
                FirstName = "New First",
                LastName = "New Last",
                Status = CustomerStatus.NonActive
            };
            // Act
            var result = await this._updateCustomerInformationHandler.Handle(request, CancellationToken.None).ConfigureAwait(false);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(request.FirstName , result.FirstName, "First name should be updated");
            Assert.AreEqual(request.LastName , result.LastName, "Last name should be updated");
            Assert.AreEqual(request.Status.Value , result.Status, "Status should be updated");
        }

        [TestMethod, Description("Customer information updated")]
        public async Task Should_ThrowPreconditionFailed_WhenRowVersionIsNotMatching()
        {
            // Arrange 
            var request = new UpdateCustomerRequest()
            {
                CustomerExternalId = this._currentExistingCustomerExternalId.ToString(),
                FirstName = "New First",
                LastName = "New Last",
                Status = CustomerStatus.NonActive,
                RowVersion = "version1"
            };
            // Act
            var result = this._rowVersionMatchValidationContractHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            var ex = await Assert.ThrowsExceptionAsync<ResourcePreconditionFailedException>(() => result).ConfigureAwait(false);
        }
    }
}
