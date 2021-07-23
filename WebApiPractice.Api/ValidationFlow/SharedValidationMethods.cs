using System.Collections.Generic;
using WebApiPractice.Api.ResponseStructure;

namespace WebApiPractice.Api.ValidationFlow
{
    public static class SharedValidationMethods
    {
        public static void ValidateStringRequired(string target, string fieldName, ref List<ErrorMessage> errorMessages)
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                errorMessages.Add(new ErrorMessage(
                    fieldName,
                    $"{fieldName} is required.")
                );
            }
        }

        public static void ValidaStringLength(string target, int maxLength, string fieldName, ref List<ErrorMessage> errorMessages)
        {
            if (target.Length > maxLength)
            {
                errorMessages.Add(new ErrorMessage(
                    fieldName,
                    $"{fieldName} must not exceed {maxLength} characters.")
                );
            }
        }
    }
}
