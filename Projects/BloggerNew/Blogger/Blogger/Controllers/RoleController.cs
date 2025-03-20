using Blogger.Data;
using Blogger.ViewModel.EditRoleViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> List()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        public IActionResult AddNewRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNewRole(ApplicationRole role)
        {
            if (role is not null)
            {
                var isExists = await _roleManager.RoleExistsAsync(role.Name);
                if (isExists)
                {
                    TempData["role_used_error"] = $"'{role.Name}' is Already Assigned.";
                    return View(role);
                }
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(List));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(result);
            }
            return BadRequest("Invalid Role!");
        }
        public async Task<IActionResult> EditRole(string id)
        {
            if (id is not null)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role is not null)
                {
                    var roleWithUsers = new EditRoleViewModel()
                    {
                        Id = role.Id,
                        Name = role.Name,
                        Description = role.Description,
                        UserNames = _userManager.GetUsersInRoleAsync(role.Id).Result.Select(x => x.UserName).ToList()
                    };

                    return View(roleWithUsers);
                }
                return RedirectToAction(nameof(List));
            }
            return BadRequest("Role id Is not Provided");
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel role)
        {
            if (role is not null)
            {
                var existingRole = await _roleManager.FindByIdAsync(role.Id);
                if (existingRole is not null)
                {
                    existingRole.Name = role.Name;
                    existingRole.Id = role.Id;
                    existingRole.Description = role.Description;
                    existingRole.ConcurrencyStamp = Guid.NewGuid().ToString();
                    var result = await _roleManager.UpdateAsync(existingRole);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(List));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(role);
                }
            }
            return BadRequest("Invalid Role!");
        }
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if(role is not null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction(nameof(List));
                }
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(List));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(role);
            }
            return BadRequest("Role Not Found!");
        }
    }
}
