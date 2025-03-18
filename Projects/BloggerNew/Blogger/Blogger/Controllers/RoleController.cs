using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
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
        public async Task<IActionResult> AddNewRole(IdentityRole role)
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
                    return View(role);
                }
                return RedirectToAction(nameof(List));
            }
            return BadRequest("Role id Is not Provided");
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(IdentityRole role)
        {
            if (role is not null)
            {
                var existingRole = await _roleManager.FindByIdAsync(role.Id);
                if (existingRole is not null)
                {
                    existingRole.Name = role.Name;
                    existingRole.Id = role.Id;
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
