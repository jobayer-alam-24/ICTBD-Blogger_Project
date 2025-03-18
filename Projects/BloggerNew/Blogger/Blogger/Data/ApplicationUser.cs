using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Blogger.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is Required"), StringLength(30, ErrorMessage = "First name must be within 30 characters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is Required"), StringLength(30, ErrorMessage = "Last name must be within 30 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Range(5, 80, ErrorMessage = "Age must be within 80 to use Blogger!")]
        public int Age { get; set; }
    }
}
