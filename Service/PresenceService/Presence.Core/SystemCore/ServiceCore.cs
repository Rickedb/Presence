using SimpleInjector;
using System;

namespace Presence.Core.SystemCore
{
    public class ServiceCore
    {
        private Container container;
        private SystemProperties serviceProperties;
        private static ServiceCore core = null;

        public static ServiceCore getInstance()
        {
            if (core != null)
                return ServiceCore.core;

            throw new Exception("Service Core was not initialized!");
        }

        public static void configure(Container container)
        {
            core = new ServiceCore(container);
        }

        public ServiceCore(Container container)
        {
            this.serviceProperties = new SystemProperties();
            this.container = container;
            core = this;
        }

        public Container Container
        {
            get { return this.container; }
            set { this.container = value; }
        }

        public SystemProperties ServiceProperties
        {
            get { return serviceProperties; }
            set { serviceProperties = value; }
        }
    }
}
