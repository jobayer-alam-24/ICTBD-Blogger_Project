using Blogger.Data;
using Blogger.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Blogger.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserController(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Details([FromRoute]string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is not null)
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
        public async Task<IActionResult> Create(ApplicationUser user, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                string imageName = "NONE";

                if (Image != null)
                {
                    string extension = Path.GetExtension(Image.FileName);
                    long size = Image.Length;
                    if (extension.Equals(".jpg") || extension.Equals(".jpeg") || extension.Equals(".png"))
                    {
                        if (size < 1000000000)
                        {
                            string path = Path.Combine(_webHostEnvironment.WebRootPath, "User", "Images", Image.FileName);
                            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                            {
                                await Image.CopyToAsync(fileStream);
                            }
                            imageName = Image.FileName;
                        }
                        else
                        {
                            TempData["size_error"] = "File must be less than 100000";
                            return View(user);
                        }
                    }
                    else
                    {
                        TempData["type_error"] = "File type should be .jpg/.png/.jpeg";
                        return View(user);
                    }
                }

                var applicationUser = new ApplicationUser()
                {
                    UserName = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Media = imageName,
                    Age = user.Age,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };

                var result = await _userManager.CreateAsync(applicationUser);
                if (result.Succeeded)
                {
                    return Redirect("~/Administrator/User/List");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser user, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByIdAsync(user.Id);
                if (existingUser != null)
                {
                    if (Image != null && !string.IsNullOrEmpty(Image.FileName))
                    {
                        string extension = Path.GetExtension(Image.FileName);
                        long size = Image.Length;
                        if (extension.Equals(".jpg") || extension.Equals(".png") || extension.Equals(".jpeg"))
                        {
                            if (size < 1000000000)
                            {
                                string path = Path.Combine(_webHostEnvironment.WebRootPath, "User", "Images", Image.FileName);
                                if (existingUser.Media != Image.FileName)
                                {
                                    string SetPath = Path.Combine(_webHostEnvironment.WebRootPath, "User", "Images", Image.FileName);
                                    using (var fileStream = new FileStream(SetPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                                    {
                                        await Image.CopyToAsync(fileStream);
                                    }

                                }
                                else
                                {
                                    using (var stream = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite, FileShare.Read))
                                    {
                                        await Image.CopyToAsync(stream);
                                    }
                                }
                            }
                            else
                            {
                                TempData["size_error"] = "File must be less than 100000";
                                return View(user);
                            }

                        }
                        else
                        {
                            TempData["type_error"] = "File type should be .jpg/.png/.jpeg";
                            return View(user);
                        }
                    }
                    existingUser.Media = Image != null && !string.IsNullOrEmpty(Image.FileName) ? Image.FileName : existingUser.Media;
                    existingUser.Email = user.Email;
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Age = user.Age;
                    existingUser.PhoneNumber = user.PhoneNumber;

                    var result = await _userManager.UpdateAsync(existingUser);
                    if (result.Succeeded)
                    {
                        return Redirect("~/Administrator/User/List");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(user);
                }
                return BadRequest("Invalid User.");
            }
            return Redirect("~/Administrator/User/List");
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id is not null)
            {
                var user = await _userManager.FindByIdAsync(id);
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "User", "Images", user.Media);
                if (!string.IsNullOrEmpty(path))
                {
                    System.IO.File.Delete(path);
                }
                await _userManager.DeleteAsync(user);
                return Redirect("~/Administrator/User/List");
            }
            return Redirect("~/Administrator/User/List");
        }

    }
}

