using System;
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

        public static void ValidateStringLength(string target, int maxLength, string fieldName, ref List<ErrorMessage> errorMessages)
        {
            if (target.Length > maxLength)
            {
                errorMessages.Add(new ErrorMessage(
                    fieldName,
                    $"{fieldName} must not exceed {maxLength} characters.")
                );
            }
        }

        public static bool IsValidExternalId(string exteralId, string fieldName, out Guid externalClientGuid, ref List<ErrorMessage> errorMessages)
        {
            if (!Guid.TryParse(exteralId, out externalClientGuid))
            {
                errorMessages.Add(new ErrorMessage(
                    fieldName,
                    $"{fieldName} must be a valid Guid.")
                );
                return false;
            }
            return true;
        }
    }
}
