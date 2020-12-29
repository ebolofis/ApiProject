using Pos_WebApi.Modules;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{
    [RoutePrefix("api/v3/da/PricelistStore")]
    public class DA_Store_PriceListAssocController : BasicV3Controller
    {
        IDA_Store_PriceListAssocFlows da_Store_PriceListAssocFlow;

        public DA_Store_PriceListAssocController(IDA_Store_PriceListAssocFlows _da_Store_PriceListAssocFlow)
        {
            this.da_Store_PriceListAssocFlow = _da_Store_PriceListAssocFlow;
        }

        /// <summary>
        /// Επιστρέφει όλες τις  pricelist ανα κατάστημα
        /// </summary>
        /// <returns>DAStore_PriceListAssocModel</returns>
        [HttpGet, Route("GetList")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetDAStore_PriceListAssoc()
        {
            List<DAStore_PriceListAssocModel> res = da_Store_PriceListAssocFlow.GetDAStore_PriceListAssoc(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}