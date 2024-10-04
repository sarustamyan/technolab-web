using System.Data.SQLite;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
            var user = GetUserFromEF(model);
            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Username), "Invalid username or password");
                ModelState.AddModelError(nameof(model.Password), "Invalid username or password");
                return View();
            }

            await PerformLogin(user);
            return LocalRedirect(returnUrl ?? "/");
        }

        private User? GetUserFromJson(LoginViewModel model)
        {
            using var context = ContextFactory.Create();

            return context.Users
                .Where(x => x.VerifyUsername(model.Username) && x.VerifyPassword(model.Password))
                .SingleOrDefault();
        }

        private User? GetUserFromEF(LoginViewModel model)
        {
            using var context = ContextFactory.Create();

            var user = context.Users
                .Where(x => x.Username == model.Username)
                .FirstOrDefault();
            if (user == null || !user.VerifyPassword(model.Password))
            {
                return null;
            }

            return user;
        }

        private User? GetUserFromSql(LoginViewModel model)
        {
            var connectionString = @"Data Source=data\data.db";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "select * from users where username = '" + model.Username + "' and PasswordHash = '" + model.Password + "'";
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }
                    return new Models.User
                    {
                        Id = (int)((long)reader["Id"]),
                        Username = (string)reader["Username"],
                        PasswordHash = (string)reader["PasswordHash"],
                        Email = (string)reader["Email"],
                        Role = (string)reader["Role"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                    };
                }
            }
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