using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WebApiPractice.Api.Extensions;

namespace WebApiPractice.Api.Tests
{
    [TestClass]
    public class ExtenstionMethodTests
    {
        [TestMethod]
        public void Should_EncodeAndDecodeBackInteger_Successefully()
        {
            // Arrange
            var value = 42;
            // Act
            var encoded = value.Base64Encode();
            var target = encoded.Base64DecodeInt();
            // Assert
            Assert.IsTrue(target == value);
        }

        [TestMethod]
        public void Should_DecodeWrongStringToZero()
        {
            // Arrange
            var value = "one";
            // Act
            var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
            var target = encoded.Base64DecodeInt();
            // Assert
            Assert.IsTrue(target == 0);
        }
    }
}

