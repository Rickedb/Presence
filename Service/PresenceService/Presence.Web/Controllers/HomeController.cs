using Presence.Core.Interfaces.Services;
using Presence.Web.Helpers;
using Presence.Web.Models;
using System.Web.Mvc;

namespace Presence.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUsersServices _userServices;

        public HomeController(IUsersServices userServices)
        {
            this._userServices = userServices;
        }

        public ActionResult Index()
        {
            var user = AuthenticationHelper.GetCurrentUsername();
            if (user != null)
                return RedirectToAction("ChooseChat", "Chat", new MappingHelper().toLoginViewModel(this._userServices.Get(user)));

            return RedirectToAction("Index", "Account");
        }
    }
}