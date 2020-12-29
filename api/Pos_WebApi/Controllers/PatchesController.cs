using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers {
    [RoutePrefix("api/Patches")]
    [AllowAnonymous]
    public class PatchesController : ApiController {

        public PatchesController( ) {
        }
        [HttpGet]
        [Route("Status")]
        public HttpResponseMessage GetStatus( ) {
            var db = new PosEntities(false);
            return this.Request.CreateResponse(System.Net.HttpStatusCode.OK, "");
        }
    }
}