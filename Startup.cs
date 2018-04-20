using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AspUploadSample.Startup))]
namespace AspUploadSample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
