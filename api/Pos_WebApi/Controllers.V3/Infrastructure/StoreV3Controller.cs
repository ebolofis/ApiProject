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
    [RoutePrefix("api/v3/Store")]
    public class StoreV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IStoreFlows storeFlow;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public StoreV3Controller(IStoreFlows st)
        {
            this.storeFlow = st;
        }

        /// <summary>
        /// Επιστρέφει τη περιγραφή του καταστήματος (τυπικά επιστρέφει μία εγγραφή) 
        /// </summary>
        /// <remarks>GET api/Store</remarks>
        /// <param name="storeid"></param>
        /// <returns>
        [HttpGet, Route("GetStores/storeid/{storeid}")]
        public HttpResponseMessage GetStores(string storeid)
        {
            StoreDetailsModel results = storeFlow.GetStores(DBInfo, storeid);
            return Request.CreateResponse(HttpStatusCode.OK, results.StoreDetailsPreview); // return results
        }
    }
}