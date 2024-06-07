using System;
using System.Net.Security;
using System.Security.Claims;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.Sqlite;
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login2(LoginViewModel model, string? returnUrl = null)
        {
            User user;
            using (var connection = new SqliteConnection(@"Data Source=d:\MyFolder\work\technolab\technolab-web\Technolab.OnlineLibrary.Web\Data\data.db"))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select * from Users where Username = '" + model.Username + "' and Password= '" + model.Password + "'";
                    var reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        user = null;
                    }
                    else
                    {
                        user = new User
                        {
                            Id = (int)(long)reader["Id"],
                            Username = (string)reader["Username"],
                            Password = (string)reader["Password"],
                            Email = (string)reader["Email"],
                            Role = (string)reader["Role"],
                            FirstName = (string)reader["FirstName"],
                            LastName = (string)reader["LastName"],
                        };
                    }
                }
            }

            if (user != null)
            {
                await PerformLogin(user);
                return LocalRedirect(returnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError(nameof(model.Username), "Invalid username or password");
                ModelState.AddModelError(nameof(model.Password), "Invalid username or password");
                return View("Login", model);
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
                string token = GenerateToken();
                user.ResetPasswordToken = BCrypt.Net.BCrypt.HashPassword(token);
                user.ResetPasswordTokenExpiry = CreateLocalTime().ToString();
                context.SaveChanges();
                var callbackUrl = Url.Action("ResetPassword", "Auth", new { token }, Request.Scheme);
                var message = CreateForgotPasswordEmailMessage(user, callbackUrl);
                EmailClient.SendEmail(message);
            }

            ViewBag.EmailSent = true;
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword2()
        {
            ViewBag.EmailSent = false;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ForgotPassword2(string username)
        {
            string pathDB = @"Data Source=C:\Users\user\Desktop\technolab-web\Technolab.OnlineLibrary.Web\Data\users.db";
            using (var connection = new SqliteConnection(pathDB))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select * from Users where Username = '" + username + "'";
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string email = (string)reader["Email"];
                        reader.Close();
                        string token = GenerateToken();
                        command.CommandText = "update Users set ResetPasswordToken = '"
                                                + BCrypt.Net.BCrypt.HashPassword(token)
                                                + "' where Username = '" + username + "'";
                        int res = command.ExecuteNonQuery();
                        command.CommandText = "update Users set ResetPasswordTokenExpiry = '"
                                                + CreateLocalTime().ToString()
                                                + "' where Username = '" + username + "'";
                        res = command.ExecuteNonQuery();
                        var callbackUrl = Url.Action("ResetPassword2", "Auth", new { token }, Request.Scheme);
                        var message = CreateForgotPasswordEmailMessage2(email, callbackUrl);
                        EmailClient.SendEmail(message);
                    }
                }
            };
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

        private DateTime CreateLocalTime()
        {
            string timeZoneId = "Caucasus Standard Time";
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTime utcNow = DateTime.UtcNow;
            return (TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone));
        }

        private bool TokenExpiry(string resetPasswordTokenExpiry)
        {
            double interval = 0.1;
            string timeFormat = "M/d/yyyy h:mm:ss tt";
            DateTime tokenCreatTime = DateTime.ParseExact(resetPasswordTokenExpiry, timeFormat, null);
            DateTime localTime = CreateLocalTime();
            return ((double)(localTime - tokenCreatTime).TotalHours < interval);
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

        private EmailMessage CreateForgotPasswordEmailMessage2(string email, string callbackUrl)
        {
            return new EmailMessage
            {
                From = "Technolab",
                To = email,
                Subject = "Reset Password",
                Body = $"To reset your password click here: {callbackUrl}"
            };
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword()
        {
            string parameterValue = HttpContext.Request.Query["token"];
            if (string.IsNullOrEmpty(parameterValue))
            {
                return RedirectToAction("ForgotPassword");
            }
            using var context = ContextFactory.Create();
            var user = context.Users.Where(x => x.ResetPasswordToken == parameterValue).SingleOrDefault();
            //&& DateTime.ParseExact(x.ResetPasswordTokenExpiry, "yyyy-MM-dd HH:mm:ss", null) > DateTime.UtcNow);
            if (user == default)
            {
                return RedirectToAction("ForgotPassword");
            }
            ViewBag.PasswordChanged = false;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ResetPassword(string username, string newPassword, string confirmPassword)
        {
            string token = HttpContext.Request.Query["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("ForgotPassword");
            }
            using var context = ContextFactory.Create();
            var user = context.Users.Where(x => x.Username == username).SingleOrDefault();
            if (user == default)
            {
                return RedirectToAction("ForgotPassword");
            }
            if (!BCrypt.Net.BCrypt.Verify(token, user.ResetPasswordToken)
                || !TokenExpiry(user.ResetPasswordTokenExpiry))
            {
                return RedirectToAction("ForgotPassword");
            }
            if (string.IsNullOrEmpty(newPassword)
                || string.IsNullOrEmpty(confirmPassword)
                || newPassword != confirmPassword)
            {
                return RedirectToAction("ForgotPassword");
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            context.SaveChanges();
            ViewBag.PasswordChanged = true;
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword2()
        {
            ViewBag.PasswordChanged = false;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ResetPassword2(string username, string newPassword, string confirmPassword)
        {
            string token = HttpContext.Request.Query["token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("ForgotPassword2");
            }
            string pathDB = @"Data Source=C:\Users\user\Desktop\technolab-web\Technolab.OnlineLibrary.Web\Data\users.db";
            using (var connection = new SqliteConnection(pathDB))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select * from Users where Username = '" + username + "'";
                    var reader = command.ExecuteReader();
                    if (!reader.Read())
                    {
                        //reader.Close();
                        return RedirectToAction("ForgotPassword2");
                    }
                    string resetPasswordToken = (string)reader["ResetPasswordToken"];
                    string resetPasswordTokenExpiry = (string)reader["resetPasswordTokenExpiry"];
                    if (!BCrypt.Net.BCrypt.Verify(token, resetPasswordToken)
                        || !TokenExpiry(resetPasswordTokenExpiry))
                    {
                        //reader.Close();
                        return RedirectToAction("ForgotPassword2");
                    }
                    if (string.IsNullOrEmpty(newPassword)
                        || string.IsNullOrEmpty(confirmPassword)
                        || newPassword != confirmPassword)
                    {
                        //reader.Close();
                        return RedirectToAction("ForgotPassword2");
                    }
                    reader.Close();
                    string password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    command.CommandText = "update Users set Password = '" + password
                                            + "' where Username = '" + username + "'";
                    int res = command.ExecuteNonQuery();
                }
            }
            ViewBag.PasswordChanged = true;
            return View();
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
    }
}