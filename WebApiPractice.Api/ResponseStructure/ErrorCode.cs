namespace WebApiPractice.Api.ResponseStructure
{
    /// <summary>
    /// Contains common error codes
    /// </summary>
    public class ErrorCode
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public ErrorCode(string code, string message)
        {
            this.Code = code;
            this.Message = message;
        }

        public static ErrorCode InternalError = new("internal",
            "Something went wrong while processing your request. We’re sorry for the trouble. We’ve been notified of the error and will correct it as soon as possible. Please try your request again in a moment.");

        public static ErrorCode Validation = new("validation",
            "Unable to process request due to validation errors.");

        public static ErrorCode InvalidRequest = new("invalid_request",
            "Invalid request.");

        public static ErrorCode ResourceNotFound = new("resource_not_found",
            "The requested resource could not be found.");

        public static ErrorCode ResourcePreconditionFailure = new("resource_precondition_failed",
            "The resource with the provided eTag does not exists.");
    }
}
