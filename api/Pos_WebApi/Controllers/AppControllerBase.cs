using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Pos_WebApi.Controllers {
    public abstract class AppControllerBase : ApiController {
        protected const string PARAM_CLIENT_L10N = "client-l10n";

        protected readonly log4net.ILog logger;
        protected PosEntities db;
        protected CultureInfo clientLocalization;

        public class ExceptionTrace : ExceptionFilterAttribute {
            public override void OnException( HttpActionExecutedContext actionExecutedContext ) {
                var logger = log4net.LogManager.GetLogger(actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerType);
                logger.Error(actionExecutedContext.Exception);
                base.OnException(actionExecutedContext);
            }
        }



        public AppControllerBase( ) {
            this.logger = log4net.LogManager.GetLogger(this.GetType());
            this.db = new PosEntities(false);
            if ( ConfigurationManager.AppSettings[PARAM_CLIENT_L10N] != null ) {
                this.clientLocalization = CultureInfo.GetCultureInfo(ConfigurationManager.AppSettings[PARAM_CLIENT_L10N]);
            }
        }
        
    }
}