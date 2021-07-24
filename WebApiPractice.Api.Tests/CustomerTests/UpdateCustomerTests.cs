using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.Resources.Customer;
using WebApiPractice.Api.Tests.TestBuilder;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Api.Tests.CustomerTests
{
    [TestClass]
    public class UpdateCustomerTests
    {
        protected Mock<ILogger<UpdateCustomerInformationHandler>> LoggerMock = LoggerHelper.GetLogger<UpdateCustomerInformationHandler>();
        private UpdateCustomerInformationHandler _updateCustomerInformationHandler;
        private Guid _currentExistingCustomerExternalId;
        private AppDbContext _appDbContex;

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
                    ContactDetails = new List<ContactDetails>()
                }
            };
            var mapper = new ObjectMapper();
            var dbContext = new AppDbContextBuilder()
                .UseInMemorySqlite()
                .WithCustomers(customers)
                .Build();
            this._appDbContex = dbContext;
            this._updateCustomerInformationHandler = new UpdateCustomerInformationHandler(this._appDbContex, mapper, LoggerMock.Object);
        }

        [TestMethod, Description("Customer information updated")]
        public async Task Should_FailValidation_WhenStatusIsNotRecognised()
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
            var result = await this._updateCustomerInformationHandler.Handle(request, CancellationToken.None);
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(request.FirstName , result.FirstName, "First name should be updated");
            Assert.AreEqual(request.LastName , result.LastName, "Last name should be updated");
            Assert.AreEqual(request.Status.Value , result.Status, "Status should be updated");
        }
    }
}
