using Blogger.ViewModel.SignUpViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.Controllers
{
    public class AuthenticationController : Controller
    {
        [Route("/sign-up")]
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [Route("/sign-up")]
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SignUp(SignUpViewModel signUpViewModel)
        {
            if(ModelState.IsValid)
            {
                return View(signUpViewModel);
            }
            return View(signUpViewModel);
        }
    }
}
