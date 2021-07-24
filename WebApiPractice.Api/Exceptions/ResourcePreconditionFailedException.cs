using System;

namespace WebApiPractice.Api.Exceptions
{
    public class ResourcePreconditionFailedException : Exception
    {
        #region Constructors
        public ResourcePreconditionFailedException() : base() { }

        public ResourcePreconditionFailedException(string message) : base(message) { }

        public ResourcePreconditionFailedException(string message, Exception innerException)
            : base(message, innerException) { }
        #endregion
    }
}
