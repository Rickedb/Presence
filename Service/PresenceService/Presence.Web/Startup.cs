using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Presence.Web.Startup))]
namespace Presence.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
