using System.ComponentModel.DataAnnotations;

namespace Blogger.ViewModel.SignInViewModel
{
    public class SignInViewModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is Required!"), StringLength(50, ErrorMessage = "Username should be less than 50 characters!"), EmailAddress]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is Required!"), StringLength(12, MinimumLength = 8, ErrorMessage = "Password must be at least 8 to 12 characters!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
