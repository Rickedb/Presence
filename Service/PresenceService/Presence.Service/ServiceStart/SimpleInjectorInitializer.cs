using SimpleInjector;
using SimpleInjector.Extensions.LifetimeScoping;

namespace Presence.Service.ServiceStart
{
    class SimpleInjectorInitializer
    {
        public static Container Initialize()
        {
            var container = new Container();

            InitializeContainer(container);

            container.Verify();

            return container;
        }

        private static void InitializeContainer(Container container)
        {
            BootStrapper.RegisterServices(container);
        }
    }
}
