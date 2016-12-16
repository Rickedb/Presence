using Presence.Service.ServiceStart;
using System.ServiceProcess;

namespace Presence.Service
{
    public partial class PresenceService : ServiceBase
    {
        public PresenceService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.initializeService();
        }

        public void initializeService()
        {
            var container = SimpleInjectorInitializer.Initialize();
            PresenceServiceInitializer.configure(container);
        }

        protected override void OnStop()
        {
            PresenceServiceInitializer.getInstance().Stop();
        }
    }
}
