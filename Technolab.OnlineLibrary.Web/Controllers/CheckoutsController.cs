using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Technolab.OnlineLibrary.Web.Controllers
{
    [Authorize(Policy = AuthorizationPolicies.Users)]
    public class CheckoutsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
