using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Technolab.OnlineLibrary.Web.Models;
using Technolab.OnlineLibrary.Web.ViewModels;

namespace Technolab.OnlineLibrary.Web.Controllers
{
    [Authorize(Policy = AuthorizationPolicies.Users)]
    public class AccountController : Controller
    {
        public AccountController(ILibraryDbContextFactory contextFactory)
        {
            this.ContextFactory = contextFactory;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (model.ConfirmNewPassword == model.NewPassword)
            {
                if (!string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    int userId = Convert.ToInt16(HttpContext.User.GetId());
                    using var context = ContextFactory.Create();
                    var user = context.Users.Find(x => x.Id == userId);

                    user.ResetPassword(model.NewPassword);
                    context.SaveChanges();

                    return View(new ChangePasswordViewModel { PasswordChangedSuccessfully = true });
                }
            } 
            else
            {
                ModelState.AddModelError(nameof(model.ConfirmNewPassword), "Passwords must match");
            }
            
            return View();
        }

        private ILibraryDbContextFactory ContextFactory { get; }
    }
}
