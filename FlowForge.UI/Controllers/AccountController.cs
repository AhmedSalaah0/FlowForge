using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Threading.Tasks;

namespace FlowForge.UI.Controllers
{
    [Controller]
    [Route("[controller]/[action]")]
    [AllowAnonymous]
    public class AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, IEmailSender sender) : Controller
    {

        [HttpGet]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Projects");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO, string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDTO);
            }
            ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                ModelState.AddModelError(nameof(LoginDTO.Email), "Invalid Email or Password");
                return View(loginDTO);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, loginDTO.RememberMe, false);
            if (result.Succeeded)
            {
                if (!ReturnUrl.IsNullOrEmpty() && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl); 
                }
                return RedirectToAction("Index", "Projects");
            }
            else
            {
                ModelState.AddModelError(nameof(LoginDTO.Email), "Invalid Email or Password");
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Projects");
            }
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
                ModelState.AddModelError(nameof(RegisterDTO.Email), "Email is already in use");
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

        [HttpGet("[action]")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("Forgot Password", "you are not register yet");
                return View();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var resetLink = $"https://flowforge.runasp.net/Account/ResetPassword?token={encodedToken}&email={email}";
            try
            {
                await sender.SendEmailAsync(email, "Reset Password", $"Please reset your password by clicking here: <a href=\"{resetLink}\">link</a>");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Email Sending Failed", ex.Message);
                return View();
            }
            ViewBag.Message = "Reset password link has been sent to your email.";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return BadRequest("Invalid user or token is expired");
            }
            var resetPasswordDTO = new ResetPasswordDTO { Email = email, token = token };
            return View(resetPasswordDTO);
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordDTO);
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "User not found");
                return View(resetPasswordDTO);
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.token, resetPasswordDTO.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(resetPasswordDTO);
            }
        }
    }
}
