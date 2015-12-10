using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdDataWebApp.Startup))]
namespace AdDataWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
