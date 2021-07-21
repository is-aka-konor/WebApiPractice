namespace WebApiPractice.Api.ValidationFlow.Interfaces
{
    /// <summary>
    /// The contract for all validations performed in API.
    /// Every single validator in the API must expose the required fields/properties via contracts.
    /// The contracts will then be implemented by requests that want to use the validation.
    /// </summary>
    public interface IValidationContract
    {
    }
}
