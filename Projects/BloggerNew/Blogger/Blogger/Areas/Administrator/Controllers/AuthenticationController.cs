using Blogger.Data;
using Blogger.ViewModel.SignInViewModel;
using Blogger.ViewModel.SignUpViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class AuthenticationController : Controller
    {
        //Services
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        //Dependency Injection
        public AuthenticationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("/sign-up")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp()
        {
            return View();
        }

        [Route("/sign-up")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Age = model.Age,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Media = "user.png"
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    TempData["success-messege"] = "Sign up Successfull!";
                    return RedirectToAction(nameof(SignIn));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

            }
            return View(model);
        }

        [Route("/sign-in")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [Route("/sign-in")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(UserViewModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        return Redirect("/");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid Sign in Attempt!");
                        return View(model);
                    }

                }

            }
            ModelState.AddModelError("", "Invalid Sign in Attempt!");
            ViewBag.ReturnUrl = ReturnUrl;
            return View(model);
        }
        [Route("/sign-out")]
        [HttpGet]
        public async Task<IActionResult> SingOut()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/sign-in");
        }
        //Used in Remote Validation
        [HttpGet]
        public async Task<IActionResult> IsEmailInUsedAsync(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user is not null)
                {
                    return Json($"{email} is already taken!");
                }
                return Json(false);
            }
            return BadRequest("Email is not Provided!");
        }
    }
}
