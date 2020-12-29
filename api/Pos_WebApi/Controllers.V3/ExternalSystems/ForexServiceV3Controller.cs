using Symposium.Models.Models;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/ForexService")]
    public class ForexServiceV3Controller : BasicV3Controller
    {
        IForexServiceFlows forexServiceFlows;

        public ForexServiceV3Controller(IForexServiceFlows forexServiceFlows)
        {
            this.forexServiceFlows = forexServiceFlows;
        }

        [HttpGet, Route("AllForex")]
        public HttpResponseMessage AllForex()
        {
            List<ForexServiceModel> res = forexServiceFlows.SelectAllForex(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
