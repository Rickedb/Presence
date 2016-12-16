using System.ServiceProcess;

namespace Presence.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            var container = ServiceStart.SimpleInjectorInitializer.Initialize();
            PresenceServiceInitializer.configure(container);
            while (true) { }
#else
                        ServiceBase[] ServicesToRun;
                        ServicesToRun = new ServiceBase[]
                        {
                            new PresenceService()
                        };
                        ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
