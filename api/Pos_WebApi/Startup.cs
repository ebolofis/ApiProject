using Microsoft.Owin;
using Owin;
using System.Web.Configuration;

[assembly: OwinStartup(typeof(Pos_WebApi.App_Start.Startup))]

namespace Pos_WebApi.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //    app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            ConfigureSignalR(app);
            
            //           ConfigureAuth(app);
        }
    }
}
