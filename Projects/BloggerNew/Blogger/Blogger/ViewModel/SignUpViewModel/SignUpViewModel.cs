using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Blogger.Helpers.CustomAttributes;

namespace Blogger.ViewModel.SignUpViewModel
{
    public class SignUpViewModel
    {
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is Required."), StringLength(50, ErrorMessage = "First Name should be less than 50 characters!")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [StringLength(50, ErrorMessage = "Last Name should be less than 50 characters!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is Required"), EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Age is Required!"), Range(3, 80, ErrorMessage = "Age should be above 3 and below 80!")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Password is Required"), DataType(DataType.Password), StringLength(12, MinimumLength = 8, ErrorMessage = "Password must be at least 8 to 12 characters!")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password"), Required(ErrorMessage = "Confirm Password is Required"), Compare("Password", ErrorMessage = "Confirm Password does not matched!"), StringLength(12, MinimumLength = 8, ErrorMessage = "Confirm Password must be at least 8 to 12 characters!")]
        public string ConfirmPassword { get; set; }
    }
}
