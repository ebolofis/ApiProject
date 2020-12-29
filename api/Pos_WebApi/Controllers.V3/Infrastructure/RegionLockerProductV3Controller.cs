
using Microsoft.AspNet.SignalR;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/RegionLockers")]
    public class RegionLockerProductV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IRegionLockersFlows regionLockers;

        public RegionLockerProductV3Controller(IRegionLockersFlows regionLockers)
        {
            this.regionLockers = regionLockers;
        }

        /// <summary>
        /// Get RegionLockers Products
        /// </summary>
        [HttpGet, Route("Product")]
        public HttpResponseMessage GetProducts()
        {
            try
            {
                List<RegionLockersModel> results = regionLockers.GetProducts(DBInfo);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }

        }
    }
}
