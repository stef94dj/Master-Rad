using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DiplomskiRad_ds130211d.Startup))]
namespace DiplomskiRad_ds130211d
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
