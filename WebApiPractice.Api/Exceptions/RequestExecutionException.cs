using System;
using System.Collections.Generic;
using WebApiPractice.Api.ResponseStructure;

namespace WebApiPractice.Api.Exceptions
{
    /// <summary>
    /// Represents validation errors that occur during application execution
    /// </summary>
    public class RequestExecutionException : Exception
    {
        public ResponseMessage ResponseMessage { get; set; } = null!;

        public RequestExecutionException(ResponseMessage responseMessage)
        {
            this.ResponseMessage = responseMessage;
        }

        public RequestExecutionException(List<ErrorMessage> messages)
        {
            this.ResponseMessage = new ResponseMessage("validation", "Validation error occurred.", messages);
        }

        #region Constructors
        public RequestExecutionException() : base() { }

        public RequestExecutionException(string message)
            : base(message) { }

        public RequestExecutionException(string message, Exception innerException)
            : base(message, innerException) { }
        #endregion
    }
}
