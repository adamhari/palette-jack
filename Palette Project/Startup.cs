using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Palette_Project.Startup))]
namespace Palette_Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
