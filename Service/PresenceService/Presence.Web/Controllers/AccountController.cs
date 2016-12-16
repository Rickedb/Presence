using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Presence.Web.Models;
using Presence.Core.Interfaces.Services;
using Presence.Web.Helpers;
using Presence.Core.Security;

namespace Presence.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly IUsersServices _userServices;
        private readonly IUsersByChatServices _userByChatServices;

        public AccountController(IUsersServices userServices, IUsersByChatServices userByChatServices)
        {
            this._userServices = userServices;
            this._userByChatServices = userByChatServices;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Index()
        {
            return View("Login");
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var user = this._userServices.Get(model.Username);
            if (model.Admin)
            {
                if (user == null)
                {
                    ViewBag.MessageTitle = "Um Erro ocorreu...";
                    ViewBag.MessageBody = "O usuário administrador " + model.Username + " não existe na base de dados!";
                    return View();
                }

                if (Encryption.decrypt(user.Password, Encryption.SYSTEM_KEY) != model.Password)
                {
                    ViewBag.MessageTitle = "Um Erro ocorreu...";
                    ViewBag.MessageBody = "Senha incorreta!";
                    return View();
                }
            }
            else
            {
                if (user == null)
                {
                    user = this._userServices.Insert(new Core.Entities.Users()
                    {
                        Username = model.Username,
                        Admin = model.Admin,
                        CreationDate = DateTime.Now
                    });
                }
                else
                    if (this._userByChatServices.getUserEnteredChats(user).Where(x => x.LastInteraction > DateTime.Now.AddMinutes(-15)).ToList().Count > 0)
                {
                    ViewBag.MessageTitle = "Um Erro ocorreu...";
                    ViewBag.MessageBody = "Usuário já esta sendo usado no momento!";
                    return View();
                }
            }
            AuthenticationHelper.RegisterAuthentication(user.Username, user.Admin, 30, true);

            return RedirectToAction("ChooseChat", "Chat", user);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult CreateUser()
        {
            if (AuthenticationHelper.GetCurrentUserAdmin())
                return View(new LoginViewModel()
                {
                    Admin = true
                });

            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult EditUser(string username)
        {
            var user = this._userServices.Get(username);
            user.Password = string.Empty;
            return View(new MappingHelper().toLoginViewModel(user));
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditUser(LoginViewModel model)
        {
            model.Password = Encryption.Encrypt(model.Password);
            var user = new MappingHelper().toUser(model);
            this._userServices.Update(user);
            ViewBag.MessageTitle = "Sucesso..";
            ViewBag.MessageBody = "Usuário alterado com sucesso!";
            return View("ShowUsers", new MappingHelper().toLoginViewModel(this._userServices.GetAll().OrderBy(x => x.Username)));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ShowUsers()
        {
            return View(new MappingHelper().toLoginViewModel(this._userServices.GetAll().OrderBy(x => x.Username)));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Delete(string username)
        {
            try
            {
                this._userServices.Delete(this._userServices.Get(username));
                ViewBag.MessageTitle = "Sucesso..";
                ViewBag.MessageBody = "Usuário < " + username + "> excluído com sucesso";
            }
            catch (Exception)
            {
                ViewBag.MessageTitle = "Um Erro Ocorreu..";
                ViewBag.MessageBody = "Usuário < " + username + " > não pode ser excluído, provavelmente ele está em algum chat neste momento ou criou um chat que ainda está ativo.";
            }
            return View("ShowUsers", new MappingHelper().toLoginViewModel(this._userServices.GetAll().OrderBy(x => x.Username)));
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}