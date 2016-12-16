using Presence.Web.Models;
using System;
using System.Web;
using System.Web.Security;

namespace Presence.Web.Helpers
{
    public static class AuthenticationHelper
    {
        private static HttpContext _context { get { return HttpContext.Current; } }

        public static void RegisterAuthentication(string name, bool admin, int timeout, bool isPersistent)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, name, DateTime.Now, DateTime.Now.AddMinutes(timeout),
                                                                             isPersistent, admin.ToString(), FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);

            _context.Response.Cookies.Add(new HttpCookie("presence", name));
            _context.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }

        public static bool GetCurrentUserAdmin()
        {
            try
            {
                return Convert.ToBoolean(GetCookie(FormsAuthentication.FormsCookieName).UserData);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retorna a chave americas do usuário logado no sistema.
        /// </summary>
        /// <returns>Username do usuário</returns>
        public static string GetCurrentUsername()
        {
            try
            {
                return GetCookie(FormsAuthentication.FormsCookieName).Name;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Descriptografa e retorna o cookie armazenado.
        /// </summary>
        /// <param name="cookieName">Nome do cookie</param>
        /// <returns>Cookie</returns>
        private static FormsAuthenticationTicket GetCookie(string cookieName)
        {
            HttpCookie authCookie = _context.Request.Cookies[cookieName];
            return FormsAuthentication.Decrypt(authCookie.Value);
        }
    }
}