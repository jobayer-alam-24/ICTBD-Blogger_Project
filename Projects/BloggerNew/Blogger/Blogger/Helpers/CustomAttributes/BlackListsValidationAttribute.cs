using System.ComponentModel.DataAnnotations;

namespace Blogger.Helpers.CustomAttributes
{
    public class BlackListsValidationAttribute : ValidationAttribute
    {
        public string[] BlackLists { get; set; }
        public BlackListsValidationAttribute(string[] blackLists)
        {
            BlackLists = blackLists;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string strValue)
            {
                foreach (var blackList in BlackLists)
                {
                    if (strValue.Contains(blackList, StringComparison.OrdinalIgnoreCase))
                    {
                        return new ValidationResult($"The value '{strValue}' contains a blacklisted term: '{blackList}'.");
                    }
                }
            }
            return ValidationResult.Success;
        }

    }
}
