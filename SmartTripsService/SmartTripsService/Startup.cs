using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SmartTripsService.Startup))]

namespace SmartTripsService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}