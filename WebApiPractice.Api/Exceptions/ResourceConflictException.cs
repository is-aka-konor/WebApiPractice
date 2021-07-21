using System;

namespace WebApiPractice.Api.Exceptions
{
    public class ResourceConflictException : Exception
    {
        #region Constructors
        public ResourceConflictException() : base() { }

        public ResourceConflictException(string message) : base(message) { }

        public ResourceConflictException(string message, Exception innerException)
            : base(message, innerException) { }
        #endregion
    }
}
