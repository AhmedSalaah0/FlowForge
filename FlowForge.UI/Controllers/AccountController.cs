using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FlowForge.UI.Controllers
{
    [Controller]
    [Route("[controller]/[action]")]
    [AllowAnonymous]
    public class AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

        [HttpGet]
        //[Authorize("NotAuthorized")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).
                    Select(temp => temp.ErrorMessage);
                return View(loginDTO);
            }
            ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                ModelState.AddModelError("Login", "Invalid Email or Password");
                return View(loginDTO);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, loginDTO.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Projects");
            }
            else
            {
                ModelState.AddModelError("Login", "Invalid Email or Password");
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDTO);
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email is already in use");
                return View(registerDTO);
            }

            var user = new ApplicationUser
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                PersonName = registerDTO.PersonName
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Projects");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerDTO);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        //[HttpPost]
        //public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(resetPasswordDTO);
        //    }
        //    var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
        //    if (user == null)
        //    {
        //        ModelState.AddModelError("Email", "User not found");
        //        return View(resetPasswordDTO);
        //    }
        //    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        //    var result = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordDTO.NewPassword);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    else
        //    {
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }
        //        return View(resetPasswordDTO);
        //    }
        //}
    }
}
