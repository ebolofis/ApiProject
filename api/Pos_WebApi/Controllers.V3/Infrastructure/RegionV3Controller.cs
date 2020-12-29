using Microsoft.AspNet.SignalR;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/Region")]
    public class RegionV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IRegionFlows regionFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public RegionV3Controller(IRegionFlows region)
        {
            this.regionFlows = region;
        }

        /// <summary>
        /// Return regions based on posinfo.Id
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="notables">not used</param>
        /// <param name="posInfoId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetRegions/storeid/{storeid}/notables/{notables}/posInfoId/{posInfoId}")]
        public HttpResponseMessage GetRegions(string storeid, bool notables, long posInfoId)
        {
            RegionModelsPreview results = regionFlows.GetRegions(DBInfo, storeid, notables, posInfoId);
            return Request.CreateResponse(HttpStatusCode.OK, results.RegionModel); // return results
        }
    }
}