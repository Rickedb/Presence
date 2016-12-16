using Presence.Core.Interfaces.Services;
using Presence.Web.Helpers;
using Presence.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presence.Web.Controllers
{
    public class ChatController : Controller
    {
        private readonly IUsersServices _userServices;
        private readonly IChatServices _chatServices;

        public ChatController(IUsersServices userServices, IChatServices chatServices)
        {
            this._userServices = userServices;
            this._chatServices = chatServices;
        }

        public ActionResult Index(int chatId)
        {
            var chat = this._chatServices.Get(chatId);
            chat.Users = this._userServices.Get(AuthenticationHelper.GetCurrentUsername());

            return View(new MappingHelper().toChatViewModel(chat));
        }

        public ActionResult ChooseChat(LoginViewModel model)
        {
            return View(model);
        }

        public ActionResult CreateChat()
        {
            return View(new ChatViewModel()
            {
                CreatorUsername = AuthenticationHelper.GetCurrentUsername(),
                Capacity = 2,
                User = new MappingHelper().toLoginViewModel(this._userServices.Get(AuthenticationHelper.GetCurrentUsername()))
            });
        }

        public ActionResult Create(ChatViewModel model)
        {
            model.User = new LoginViewModel() { Username = AuthenticationHelper.GetCurrentUsername() };
            model.CreatorUsername = model.User.Username;
            model.CreationDate = DateTime.Now;

            if (this._chatServices.GetAll().Where(x=> x.Name == model.Name).ToList().Count > 0)
            {
                ViewBag.MessageTitle = "Um Erro ocorreu...";
                ViewBag.MessageBody = "Já existe um chat ativo com este nome!";
                return View("CreateChat", model);
            }
            
            var chat = this._chatServices.Insert(new MappingHelper().toChat(model));
            return RedirectToAction("Index", "Chat", new { chatId = chat.ID });
        }
    }
}