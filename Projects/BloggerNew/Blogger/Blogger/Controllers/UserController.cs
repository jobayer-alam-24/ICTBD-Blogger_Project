using Blogger.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> List()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user is not null)
            {
                return View(user);
            }
            return BadRequest("User Not Found!");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id != null)
            {
                var found = await _userManager.FindByIdAsync(id);
                if (found != null)
                {
                    return View(found);
                }
            }
            return BadRequest("User Not Found!");
        }
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser()
                {
                    UserName = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Age = user.Age,
                    Email = user.Email
                };
                var result = await _userManager.CreateAsync(applicationUser);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(List));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(user);
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var getUser = await _userManager.FindByIdAsync(user.Id);
                if (getUser != null)
                {
                    getUser.Email = user.Email;
                    getUser.FirstName = user.FirstName;
                    getUser.LastName = user.LastName; 
                    getUser.Age = user.Age;

                    var result = await _userManager.UpdateAsync(getUser);
                    if(result.Succeeded)
                    {
                        return RedirectToAction(nameof(List));
                    }
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(user);
                }
                return BadRequest("Invalid User.");
            }
            return RedirectToAction(nameof(List));
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id is not null)
            {
                var user = await _userManager.FindByIdAsync(id);
                await _userManager.DeleteAsync(user);
                return RedirectToAction(nameof(List));
            }
            return RedirectToAction(nameof(List));
        }

    }
}

