using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Api.Enumerations;
using WebApiPractice.Api.Resources.Customers;
using WebApiPractice.Api.Resources.Customers.Validations;

namespace WebApiPractice.Api.Tests.CustomerTests
{
    [TestClass]
    public class CustomerInformationTests
    {
        private readonly CustomerInformationValidationHandler _customerInformationValidationHandler = new ();

        [TestMethod, Description("Validate required fields")]
        public async Task Should_FailValidation_WhenAnyOfRequiredFieldsAreEmpty()
        {
            // Arrange 
            var request = new PostCustomerRequest();
            // Act
            var result = await _customerInformationValidationHandler.Handle(request, CancellationToken.None).ConfigureAwait(false);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("FirstName is required")), "FirstName cannot be empty or null");
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("LastName is required")), "LastName cannot be empty or null");
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("Status is not recognized")), "Status cannot be Unknown");
        }

        [TestMethod, Description("Validate fields length")]
        public async Task Should_FailValidation_WhenDoNotPassLenthValidation()
        {
            // Arrange 
            var request = new PostCustomerRequest()
            {
                FirstName = new string('a', 31),
                LastName = new string('a', 71),
                Status = CustomerStatus.Current
            };
            // Act
            var result = await _customerInformationValidationHandler.Handle(request, CancellationToken.None).ConfigureAwait(false);
            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("FirstName must not exceed 30")), "FirstName cannot be longer than 30 characters");
            Assert.IsTrue(result.Any(mes => mes.Description.Contains("LastName must not exceed 70")), "LastName cannot be longer than 70 characters");
        }
    }
}
