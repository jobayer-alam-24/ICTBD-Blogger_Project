using Blogger.ViewModel.SignInViewModel;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(SignUpViewModel signUpViewModel)
        {
            if(ModelState.IsValid)
            {
                return View(signUpViewModel);
            }
            return View(signUpViewModel);
        }

        [Route("/sign-in")]
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [Route("/sign-in")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(SignInViewModel signInViewModel)
        {
            if(ModelState.IsValid)
            {
                return View(signInViewModel);
            }
            return View(signInViewModel);
        }
    }
}
