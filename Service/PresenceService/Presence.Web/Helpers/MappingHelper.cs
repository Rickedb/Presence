using Presence.Core.Entities;
using Presence.Web.Models;
using System.Collections.Generic;

namespace Presence.Web.Helpers
{
    public class MappingHelper
    {
        public ChatViewModel toChatViewModel(Chat chat)
        {
            return new ChatViewModel()
            {
                ID = chat.ID,
                User = this.toLoginViewModel(chat.Users),
                Name = chat.Name,
                CreatorUsername = chat.CreatorUsername,
                CreationDate = chat.CreationDate,
                Capacity = chat.Capacity,
                EndDate = chat.EndDate
            };

        }

        public LoginViewModel toLoginViewModel(Users user)
        {
            return new LoginViewModel()
            {
                Username = user.Username,
                Admin = user.Admin,
                Password = user.Password,
                CreationDate = user.CreationDate
            };

        }

        public List<LoginViewModel> toLoginViewModel(IEnumerable<Users> users)
        {
            List<LoginViewModel> logins = new List<LoginViewModel>();
            foreach (Users user in users)
                logins.Add(this.toLoginViewModel(user));
            return logins;
        }

        public Chat toChat(ChatViewModel chat)
        {
            return new Chat()
            {
                ID = chat.ID,
                Users = this.toUser(chat.User),
                Name = chat.Name,
                CreatorUsername = chat.CreatorUsername,
                CreationDate = chat.CreationDate,
                Capacity = chat.Capacity,
                EndDate = chat.EndDate
            };
        }

        public Users toUser(LoginViewModel user)
        {
            return new Users()
            {
                Username = user.Username,
                Admin = user.Admin,
                Password = user.Password,
                CreationDate = user.CreationDate
            };

        }
    }
}