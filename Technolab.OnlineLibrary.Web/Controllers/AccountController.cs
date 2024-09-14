using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Technolab.OnlineLibrary.Web.Controllers
{
    [Authorize(Policy = AuthorizationPolicies.Users)]
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
    }
}
