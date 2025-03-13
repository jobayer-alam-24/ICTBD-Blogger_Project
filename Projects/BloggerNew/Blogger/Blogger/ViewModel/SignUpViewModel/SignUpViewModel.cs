using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Password is Required"), DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Confirm Password"), Required(ErrorMessage = "Confirm Password is Required"), Compare("Password", ErrorMessage = "Confirm Password does not matched!")]
        public string ConfirmPassword { get; set; }
    }
}
