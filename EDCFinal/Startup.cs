using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EDCFinal.Startup))]
namespace EDCFinal
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
