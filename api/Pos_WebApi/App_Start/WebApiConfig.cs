using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Pos_WebApi.Helpers;
using System.Web.Http.Cors;
using Pos_WebApi.Modules;
using Pos_WebApi.Controllers;
using System.Web.Http.ExceptionHandling;

namespace Pos_WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Filters.Add(new AuthorizeAttribute());

            config.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());
           
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            config.MessageHandlers.Add(new TraceHandler());

        }
    }
}
