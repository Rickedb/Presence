using Presence.Core.Context;
using Presence.Core.Interfaces.Repository;
using Presence.Core.Interfaces.Services;
using Presence.Core.Repository;
using Presence.Core.Services;

using SimpleInjector;

namespace Presence.Service.ServiceStart
{
    public class BootStrapper
    {
        public static void RegisterServices(Container container)
        {
            container.Register<IChatServices, ChatServices>();
            container.Register<IMessagesServices, MessagesServices>();
            container.Register<IUsersByChatServices, UsersByChatServices>();
            container.Register<IUsersServices, UsersServices>();

            container.Register<IChatRepository, ChatRepository>();
            container.Register<IMessagesRepository, MessagesRepository>();
            container.Register<IUsersByChatRepository, UsersByChatRepository>();
            container.Register<IUsersRepository, UsersRepository>();

            container.Register<PresenceContext>();

            container.GetRegistration(typeof(IChatServices)).Registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Workaround");
            container.GetRegistration(typeof(IMessagesServices)).Registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Workaround");
            container.GetRegistration(typeof(IUsersByChatServices)).Registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Workaround");
            container.GetRegistration(typeof(IUsersServices)).Registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Workaround");

            container.GetRegistration(typeof(IChatRepository)).Registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Workaround");
            container.GetRegistration(typeof(IMessagesRepository)).Registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Workaround");
            container.GetRegistration(typeof(IUsersByChatRepository)).Registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Workaround");
            container.GetRegistration(typeof(IUsersRepository)).Registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Workaround");

            container.GetRegistration(typeof(PresenceContext)).Registration.SuppressDiagnosticWarning(SimpleInjector.Diagnostics.DiagnosticType.DisposableTransientComponent, "Workaround");
        }
    }
}
