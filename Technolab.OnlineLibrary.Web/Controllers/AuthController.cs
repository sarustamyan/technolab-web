using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NuGet.Common;
using Technolab.OnlineLibrary.Web.Models;
using Technolab.OnlineLibrary.Web.ViewModels;

namespace Technolab.OnlineLibrary.Web.Controllers
{
    public class AuthController : Controller
    {
        public AuthController(ILibraryDbContextFactory contextFactory, IEmailClient emailClient)
        {
            this.ContextFactory = contextFactory;
            this.EmailClient = emailClient;
            this.token = null;
            this.tokenExpiry = null;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            using var context = ContextFactory.Create();

            var user = context.Users
                .Where(x => x.Username == model.Username)
                .SingleOrDefault();

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                await PerformLogin(user);
                return LocalRedirect(returnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError(nameof(model.Username), "Invalid username or password");
                ModelState.AddModelError(nameof(model.Password), "Invalid username or password");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ViewBag.EmailSent = false;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ForgotPassword(string username)
        {
            using var context = ContextFactory.Create();

            var user = context.Users.Where(x => x.Username == username).SingleOrDefault();

            if (user != default)
            {
                token = GenerateToken();
                tokenExpiry = DateTime.UtcNow.AddHours(1);
                var callbackUrl = Url.Action("ResetPassword", "Auth", new { token }, Request.Scheme);
                var message = CreateForgotPasswordEmailMessage(user, callbackUrl);
                EmailClient.SendEmail(message);

            }

            ViewBag.EmailSent = true;
            return View();
        }

        private string GenerateToken()
        {
            byte[] tokenBytes = new byte[32];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenBytes);
            }
            return WebEncoders.Base64UrlEncode(tokenBytes);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string urlToken)
        {
            Console.WriteLine($"URLTOKEN:***{urlToken}***");
            Console.WriteLine($"TOKEN:***{token}***");
            if (string.IsNullOrEmpty(urlToken) || urlToken == token)
            {
                ViewBag.PasswordChanged = false;
                //return RedirectToAction("ForgotPassword");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            if (newPassword == "hello" && confirmPassword == "world")
            {
                ViewBag.PasswordChanged = true;
            }
            return View();
        }



        private EmailMessage CreateForgotPasswordEmailMessage(User user, string callbackUrl)
        {
            return new EmailMessage
            {
                From = "Technolab",
                To = user.Email,
                Subject = "Reset Password",
                Body = $"To reset your password click here: {callbackUrl}"
            };
        }

        private async Task PerformLogin(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(type: Consts.Claim.UserId, value: user.Id.ToString()),
                new Claim(type: Consts.Claim.Username, value: user.Username),
                new Claim(type: Consts.Claim.UserRole, value: user.Role),
                new Claim(type: Consts.Claim.Email, value: user.Email),
                new Claim(type: Consts.Claim.FirstName, value: user.FirstName),
                new Claim(type: Consts.Claim.LastName, value: user.LastName),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        private ILibraryDbContextFactory ContextFactory { get; }
        private IEmailClient EmailClient { get; }

        private string token;
        private DateTime? tokenExpiry;
    }
}