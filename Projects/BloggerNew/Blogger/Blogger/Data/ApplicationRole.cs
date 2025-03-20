using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Blogger.Data
{
    public class ApplicationRole : IdentityRole
    {
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Role Description is Required.")]
        public string Description { get; set; }
    }
}
