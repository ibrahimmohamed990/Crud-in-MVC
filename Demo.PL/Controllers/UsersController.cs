using Demo.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.PL.Controllers
{
    [Authorize(Roles = "Admin, HR")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<UsersController> logger;

        public UsersController(UserManager<ApplicationUser> _userManager,
            ILogger<UsersController> _logger)
        {
            userManager = _userManager;
            logger = _logger;
        }
        public async Task<IActionResult> Index(string SearchValue = "")
        {
            List<ApplicationUser> users;
            if (string.IsNullOrEmpty(SearchValue))
                users = await userManager.Users.ToListAsync();
            else
            {
                users = await userManager.Users.Where(x =>
                x.NormalizedEmail.Trim().Contains(SearchValue.Trim().ToUpper()) ||
                x.NormalizedUserName.Trim().Contains(SearchValue.Trim().ToUpper())).ToListAsync();
                ViewBag.SearchValue = SearchValue;
            }
            return View(users);
        }
        public async Task<IActionResult> Details(string id)
        {
            if (id is null)
                return NotFound();

            var users = await userManager.FindByIdAsync(id);
            bool IsActiveCheck = (users.IsActive == true);
            TempData["IsActive"] = IsActiveCheck.ToString();
            
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            if (id is null)
                return NotFound();
            var user = await userManager.FindByIdAsync(id);

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Update(string id, ApplicationUser input)
        {
            if (input is null || id is null)
                return NotFound();
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(id);
                user.UserName = input.UserName;
                user.NormalizedUserName = input.UserName.ToUpper();
                var result = await userManager.UpdateAsync(user);
                if(result.Succeeded)
                    return RedirectToAction("Index");
                foreach (var error in result.Errors)
                {
                    logger.LogError(error.Description);
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(input);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if(id is null) return NotFound();
            var user = await userManager.FindByIdAsync(id);

            var result = await userManager.DeleteAsync(user);
            if(result.Succeeded)
                return RedirectToAction("Index");
            foreach (var error in result.Errors)
            {
                logger.LogError(error.Description);
                ModelState.AddModelError("", error.Description);
            }
            return NotFound();
        }
    }
}
