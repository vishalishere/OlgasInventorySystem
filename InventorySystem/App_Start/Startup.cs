using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(InventorySystem.App_Start.Startup))]

namespace InventorySystem.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
