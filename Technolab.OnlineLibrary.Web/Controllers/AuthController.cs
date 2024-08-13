using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Technolab.OnlineLibrary.Web.Models;
using Technolab.OnlineLibrary.Web.ViewModels;

namespace Technolab.OnlineLibrary.Web.Controllers
{
    public class AuthController : Controller
    {
        public AuthController(ILibraryDbContextFactory contextFactory)
        {
            this.ContextFactory = contextFactory;
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
                .Where(x => x.Username == model.Username && x.Password == model.Password)
                .SingleOrDefault();
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Username), "Invalid username or password");
                ModelState.AddModelError(nameof(model.Password), "Invalid username or password");
                return View();
            }

            await PerformLogin(user);
            return LocalRedirect(returnUrl ?? "/");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
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
    }
}
