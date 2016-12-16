[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Presence.Web.App_Start.BootStrapper), "Initialize")]

namespace Presence.Web.App_Start
{
    using SimpleInjector;
    using Presence.Core.Context;
    using Presence.Core.Interfaces.Services;
    using Presence.Core.Services;
    using Presence.Core.Interfaces.Repository;
    using Presence.Core.Repository;
    using SimpleInjector.Integration.Web;
    using System.Reflection;
    using System.Web.Mvc;
    using SimpleInjector.Integration.Web.Mvc;

    public class BootStrapper
    {
        /// <summary>
        /// Inicia processo de injeção de dependencia por Web Request
        /// </summary>
        public static void Initialize()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            RegisterServices(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        /// <summary>
        /// Efetua a Injeção de Dependência
        /// </summary>
        /// <param name="container"></param>
        private static void RegisterServices(Container container)
        {
            container.RegisterPerWebRequest<IChatRepository, ChatRepository>();
            container.RegisterPerWebRequest<IMessagesRepository, MessagesRepository>();
            container.RegisterPerWebRequest<IUsersRepository, UsersRepository>();
            container.RegisterPerWebRequest<IUsersByChatRepository, UsersByChatRepository>();

            container.RegisterPerWebRequest<IChatServices, ChatServices>();
            container.RegisterPerWebRequest<IMessagesServices, MessagesServices>();
            container.RegisterPerWebRequest<IUsersServices, UsersServices>();
            container.RegisterPerWebRequest<IUsersByChatServices, UsersByChatServices>();

            container.RegisterPerWebRequest<PresenceContext>();
        }
    }
}