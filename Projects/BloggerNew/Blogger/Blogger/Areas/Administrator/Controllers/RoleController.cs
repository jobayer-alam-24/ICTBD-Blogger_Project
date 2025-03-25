using Blogger.Data;
using Blogger.ViewModel.EditRoleViewModel;
using Blogger.ViewModel.UserRoleViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = "Admin,Super Admin")]
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
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> EditRole(string roleid)
        {

            var role = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == roleid);
            if (role is not null)
            {
                var roleWithUsers = new EditRoleViewModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    UserNames = _userManager.GetUsersInRoleAsync(role.Name).Result.Select(x => x.Email).ToList()
                };

                return View(roleWithUsers);
            }
            return RedirectToAction(nameof(List));

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(EditRoleViewModel role)
        {
            if (role is not null)
            {
                var existingRole = await _roleManager.Roles.FirstOrDefaultAsync(x => x.Id == role.Id);
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
        [Route("/AssginUserRole")]
        public async Task<IActionResult> AssignUserRole(string roleid)
        {
            if (roleid == null) return BadRequest("Role Id is not Provided!");
            var role = await _roleManager.FindByIdAsync(roleid);
            if (role is not null)
            {
                ViewBag.RoleId = role.Id;
                ViewBag.RoleName = role.Name;
                List<UserRoleViewModel> userRoleViewModels = new List<UserRoleViewModel>();
                foreach (var user in _userManager.Users)
                {
                    var userRole = new UserRoleViewModel()
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        IsSelected = await _userManager.IsInRoleAsync(user, role.Name) ? true : false
                    };
                    userRoleViewModels.Add(userRole);
                }
                return View(userRoleViewModels);
            }
            return BadRequest("Role Not Found");
        }
        [Route("/AssginUserRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUserRole(string roleid, List<UserRoleViewModel> models)
        {
            if (string.IsNullOrWhiteSpace(roleid)) return BadRequest("Role id is not Provided");
            var role = await _roleManager.FindByIdAsync(roleid);
            var result = new IdentityResult();
            int length = models.Count;
            for (int i = 0; i < length; i++)
            {
                var user = await _userManager.FindByIdAsync(models[i].UserId);

                if (models[i].IsSelected && !await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!models[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else { continue; }
            }
            if (result.Succeeded)
            {
                return RedirectToAction("EditRole", new { roleid = role.Id });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(models);
            }

        }

        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is not null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
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
