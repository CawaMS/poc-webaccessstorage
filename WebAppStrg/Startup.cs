using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAppStrg.Startup))]
namespace WebAppStrg
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
