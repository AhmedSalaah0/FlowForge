using FlowForge.Core.Domain.IdentityEntities;
using FlowForge.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace FlowForge.UI.Controllers
{
    [Controller]
    [Route("[controller]/[action]")]
    public class AccountController(UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager, IEmailSender sender) : Controller
    {

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Projects");
            }
            return View();
        }

        [HttpPost]
        [Authorize("NotAuthenticated")]
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
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Projects");
            }
            return View();
        }

        [HttpPost]
        [Authorize("NotAuthenticated")]
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
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize("NotAuthenticated")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize("NotAuthenticated")]
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
            ViewBag.Message = "Reset password link has been sent to your email(Check spam box).";
            return View();
        }

        [HttpGet]
        [Authorize("NotAuthenticated")]
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
        [Authorize("NotAuthenticated")]
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

        [HttpGet]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            return PartialView("_ProfilePartialView", user);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordDTO);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                ViewBag.Message = "Your password has been changed.";
                return View();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(changePasswordDTO);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangeEmail()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Unauthorized();
            }

            return View(new ChangeEmailDTO
            {
                CurrentEmail = user.Email,
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeEmailDTO changeEmailDTO)
        {
            if (string.IsNullOrEmpty(changeEmailDTO.NewEmail) || !new EmailAddressAttribute().IsValid(changeEmailDTO.NewEmail))
            {
                ModelState.AddModelError("Email", "Invalid email address");
                return View();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var existingUser = await _userManager.FindByEmailAsync(changeEmailDTO.NewEmail);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email is already in use");
                return View();
            }
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, changeEmailDTO.NewEmail);
            var encodedToken = WebUtility.UrlEncode(token);
            var confirmationLink = Url.Action("ConfirmEmailChange", "Account", new { userId = user.Id, email = changeEmailDTO.NewEmail, token = encodedToken }, Request.Scheme);
            try
            {
                await sender.SendEmailAsync(changeEmailDTO.NewEmail, "Confirm your email change", $"Please confirm your email change by clicking here: <a href=\"{confirmationLink}\">link</a>");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Email Sending Failed", ex.Message);
                return View();
            }
            ViewBag.Message = "A confirmation link has been sent to your new email address. Please check your email to confirm the change (Check spam box).";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string token)
        {
            if (userId == null || email == null || token == null)
            {
                return BadRequest("Invalid user or token is expired");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }
            var decodedToken = WebUtility.UrlDecode(token);

            var isValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.ChangeEmailTokenProvider, "ChangeEmail", decodedToken);
            if (!isValid)
            {
                throw new KeyNotFoundException("Invalid token");
            }

            var ChangeEmail = await _userManager.ChangeEmailAsync(user, email, decodedToken);
            var SetUserName = await _userManager.SetUserNameAsync(user, email);
            if (ChangeEmail.Succeeded && SetUserName.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                ViewBag.Message = "Your email has been changed.";
                return View();
            }
            else
            {
                foreach (var error in ChangeEmail.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                foreach (var error in SetUserName.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
        }
    }
}
