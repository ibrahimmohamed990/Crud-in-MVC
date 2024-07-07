using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.PL.Controllers
{
    [Authorize(Roles = "Admin, HR")]
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<RolesController> logger;

        public RolesController(RoleManager<ApplicationRole> _roleManager,
            UserManager<ApplicationUser> _userManager,
            ILogger<RolesController> _logger)
        {
            roleManager = _roleManager;
            userManager = _userManager;
            logger = _logger;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return View(roles);
        }
        public async Task<IActionResult> Create()
        {
            return View(new ApplicationRole());
        }
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationRole input)
        {
            if(ModelState.IsValid)
            {
                var result = await roleManager.CreateAsync(input);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                foreach (var error in result.Errors)
                {
                    logger.LogError(error.Description);
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(input);
        }
        public async Task<IActionResult> Details(string id)
        {
            if(id is null) return NotFound();
            var role = await roleManager.FindByIdAsync(id);
            if(role is null) return NotFound();
            return View(role);
        }
        public async Task<IActionResult> Update(string id)
        {
            if(id is null) return NotFound();
            var role = await roleManager.FindByIdAsync(id);
            if(role is null) return NotFound();
            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> Update(ApplicationRole input)
        {
            if(input is null) return NotFound();
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(input.Id);
                if(role is null) return NotFound();
                role.Name = input.Name;
                var result = await roleManager.UpdateAsync(role);
                if(result.Succeeded) 
                    return RedirectToAction("Index");
                else 
                    return View(input);
            }
            return View(input);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id is null) return NotFound();
            var role = await roleManager.FindByIdAsync(id);
            if(role is null) return NotFound();
            var result = await roleManager.DeleteAsync(role);
            if(result.Succeeded)
                return RedirectToAction("Index");
            return View(result);

        }
        public async Task<IActionResult> AddOrRemoveUsers(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if (role is null) return NotFound();
            var usersInRole = new List<UserInRoleViewModel>();
            var users = await userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                var userInRole = new UserInRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                ViewBag.RoleName = role.Name; 

                if (await userManager.IsInRoleAsync(user, role.Name))
                    userInRole.IsSelected = true;
                else
                    userInRole.IsSelected = false;

                usersInRole.Add(userInRole);
            }
            return View(usersInRole);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(string Id, List<UserInRoleViewModel> users)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if(role is null) return NotFound();
            if (ModelState.IsValid)
            {
                foreach(var user in users)
                {
                    var appUser = await userManager.FindByIdAsync(user.UserId);
                    if (appUser is null) return NotFound();

                    if (user.IsSelected && !await userManager.IsInRoleAsync(appUser, role.Name))
                        await userManager.AddToRoleAsync(appUser, role.Name);
                    else if(!user.IsSelected && await userManager.IsInRoleAsync(appUser,role.Name))
                        await userManager.RemoveFromRoleAsync(appUser, role.Name);
                }
                return RedirectToAction("Index");
            }
            return View(users);
        }

    }
}
