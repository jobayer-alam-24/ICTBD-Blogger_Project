using System.ComponentModel.DataAnnotations;

namespace Blogger.ViewModel.SignInViewModel
{
    public class UserViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is Required!"), StringLength(50, ErrorMessage = "Email should be less than 50 characters!"), EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is Required!"), StringLength(12, MinimumLength = 6, ErrorMessage = "Password must be at least 6 to 12 characters!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
