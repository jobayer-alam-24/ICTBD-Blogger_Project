using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Blogger.Helpers.CustomAttributes
{
    public class WhiteListsValidationAttribute : ValidationAttribute
    {
        public string[] WhiteLists { get; set; }

        public WhiteListsValidationAttribute(string[] whitelists)
        {
            WhiteLists = whitelists;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string strValue)
            {
                if (WhiteLists.Contains(strValue))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"The value '{strValue}' is not in the allowed list.");
                }
            }
            return new ValidationResult("Invalid data type.");
        }
    }
}
