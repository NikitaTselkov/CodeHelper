using CodeHelper.Core;
using CodeHelper.Models.Domain;
using CodeHelper.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeHelper.Controllers
{
    public class AutorizationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AutorizationController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["CurrentPage"] = "Autorization";
            ViewData["ReturnUrl"] = returnUrl;

            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["CurrentPage"] = "Autorization";

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);

                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

                    if (result.Succeeded)
                    {
                        if (Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return RedirectToAction("All", "Questions");
                    }
                }
            }

            ModelState.AddModelError(nameof(model.Password), "Password is invalid");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignUp(string? returnUrl = null)
        {
            ViewData["CurrentPage"] = "Autorization";
            ViewData["ReturnUrl"] = returnUrl;

            var response = new SignUpViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model, string? returnUrl = null)
        {
            ViewData["CurrentPage"] = "Autorization";

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                user = new User(model);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

                    if (result.Succeeded)
                    {
                        if (Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return RedirectToAction("All", "Questions");
                    }
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            else
            {
                ModelState.AddModelError(nameof(model.Email), "Email is already in use");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            Response.Cookies.Delete(GlobalConstants.AuthCookieName);

            return RedirectToAction("All", "Questions");
        }

        [HttpGet]
        public IActionResult ForgetPassword(ForgetPasswordViewModel model)
        {
            ViewData["CurrentPage"] = "Autorization";

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(ForgetPasswordViewModel model)
        {
            return View(new ResetPasswordViewModel());
        }
    }
}
