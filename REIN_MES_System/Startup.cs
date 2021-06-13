using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ZHB_AD.Startup))]
namespace ZHB_AD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
