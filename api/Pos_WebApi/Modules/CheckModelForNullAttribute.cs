using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Pos_WebApi.Modules
{
    /// <summary>
    /// Action Filter that checks the action arguments to find out whether any of them has been passed as null. 
    /// If that’s true, you reject the request with a 400 response
    /// 
    /// In order for the Actions to be validated, must be decorated with the attribute: [CheckModelForNull]
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CheckModelForNullAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ActionArguments.ContainsValue(null))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                        HttpStatusCode.BadRequest,
                            string.Format(Symposium.Resources.Errors.NULLARGUMENT, string.Join(",",
                                            actionContext.ActionArguments.Where(i => i.Value == null).Select(i => i.Key))));
            }
        }
    }


}