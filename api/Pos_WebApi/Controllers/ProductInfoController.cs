using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/ProductInfo")]
    public class ProductInfoController : ApiController
    {
        /// <summary>
        /// Return the product version
        /// </summary>
        /// <returns>version as x.x.x.x</returns>
        [Route("version")]
        public string GetVersion()
        {
            var p = this.GetType().Assembly.GetName().Version;
            var r = string.Format("{0}.{1}.{2}.{3}", p.Major, p.Minor, p.Build, p.Revision);
            return r;
        }

        [Route("apiversion")]
        public string GetApiVersion()
        {
            var p = this.GetType().Assembly.GetName().Version;
            var r = string.Format("{0}.{1}.{2}.{3}", p.Major, p.Minor, p.Build,p.Revision);
            return r;
        }
    }
}
