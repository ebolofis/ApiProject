using log4net;
using Symposium.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

namespace Pos_WebApi.Helpers
{
    /// <summary>
    /// Replaced by TraceExceptionLogger (see bellow)
    /// </summary>
    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void OnException(HttpActionExecutedContext context)
        {
            try {
              //  Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(context.Exception));
            } catch(Exception ex) {
                logger.Error(ex.ToString());
            }
        }
    }

    /// <summary>
    /// catch all unhandled exceptions
    /// </summary>
    public class TraceExceptionLogger : ExceptionLogger
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // public override async Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        public override  void Log(ExceptionLoggerContext context)
        {
            string requestBody = string.Empty;
            string debugInfo = "";
            using (var sr = new StreamReader(HttpContext.Current.Request.InputStream))
            {
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                requestBody = sr.ReadToEnd();
            }


            
            //string requestBody =  context.Request.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrWhiteSpace(requestBody))
                requestBody = requestBody.Replace("\\r\\n", " ").Replace("\n", " ");
            else
                requestBody = "";

            if (context.Exception is BusinessException)
            {
                string exception = context.Exception.Message;
                debugInfo = string.Format("Response with Business Exception : [{0}]  [{1}] {2} ", context.Request.Method, context.Request.RequestUri, exception);
                if (requestBody != "") debugInfo = debugInfo + Environment.NewLine + "  REQUEST BODY: " + requestBody;
                logger.Warn(debugInfo);
                logger.Debug(" The full Trace for the previous Business Exception is: " + context.Exception.ToString());
            }
            else
            {
                string exheption = context.Exception.ToString();
                exheption = Environment.NewLine + exheption;
                //string help = Environment.NewLine + "  THE TRUE REASON OF THE ERROR MAY BE LOGGED IN A PREVIOUS LINE INTO LOG FILE. Check for previous errors as well !!!" + Environment.NewLine;
                //string line = "=======================================================================";
                debugInfo = string.Format("Response : [{0}]  [{1}] {2} ",
                    context.Request.Method, context.Request.RequestUri, exheption);
                if (requestBody != "") debugInfo = debugInfo + Environment.NewLine + Environment.NewLine + "  REQUEST BODY: " + requestBody;
                logger.Error(debugInfo);
            }

      

        }
    }
}