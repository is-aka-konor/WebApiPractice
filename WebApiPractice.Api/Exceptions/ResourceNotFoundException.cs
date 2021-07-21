using System;

namespace WebApiPractice.Api.Exceptions
{
    /// <summary>
    /// Exception indicating a resource could not be found
    /// </summary>
    public class ResourceNotFoundException : Exception
    {
        #region Constructors
        public ResourceNotFoundException() : base() { }

        public ResourceNotFoundException(string message) : base(message) { }

        public ResourceNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
        #endregion
    }
}
