using Demo.DAL.Entities;
using Demo.PL.Helpers;
using Demo.PL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager,
            ILogger<AccountController> _logger)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            logger = _logger;
        }

        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = input.Email,
                    UserName = input.Email.Split("@")[0],
                    IsActive = true
                };
                var result = await userManager.CreateAsync(user, input.Password);
                if (result.Succeeded)
                    return RedirectToAction("SignIn");
                List<string> errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                    logger.LogError(error.Description);
                    ModelState.AddModelError("", error.Description);
                }
                ViewBag.Errors = errors;
                //return View(input);
            }
            return View(input);
        }
        [HttpGet]
        public IActionResult SignIn()
        {
            return View(new SignInViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(input.Email);
                if (user is null)
                    ModelState.AddModelError("email", "Email does not exist.");
                
                if (user != null && await userManager.CheckPasswordAsync(user, input.Password))
                {
                    var result = await signInManager.PasswordSignInAsync(user, input.Password, input.RememberMe, false);
                    if (result.Succeeded)
                        return RedirectToAction("Index", "Home");
                }
                else if((user != null && !(await userManager.CheckPasswordAsync(user, input.Password))))
                    ModelState.AddModelError("", "Incorrect Password!!");

            }
            return View(input);
        }

        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("SignIn");
        }

        public IActionResult ForgetPassword()
        {
            return View(new ForgetPasswordViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(input.Email);
                if (user is null)
                    ModelState.AddModelError("", "Email does not Exist!!");
                if(user != null)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    
                    var resetPassword = Url.Action("ResetPassword", "Account",  new { Email = input.Email , Token = token }, "https");
                    var email = new Email
                    {
                        Tittle = "Reset Password",
                        Body = resetPassword??"",
                        To = input.Email
                    };
                    // Send Email
                    EmailSettings.SendEmail(email);

                    return RedirectToAction("CompleteForgetPassword");
                }
            }
            return View(input);
        }
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel input)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(input.Email);

                if( user is null)
                    ModelState.AddModelError("", "Email does not exist.");
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, input.Token, input.Password);
                    if (result.Succeeded)
                        return RedirectToAction("SignIn");
                    foreach (var error in result.Errors)
                    {
                        logger.LogError(error.Description);
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(input);
        }

        public IActionResult CompleteForgetPassword()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
