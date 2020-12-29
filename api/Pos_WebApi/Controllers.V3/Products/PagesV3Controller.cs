using Microsoft.AspNet.SignalR;
using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/Pages")]
    public class PagesV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IPagesFlows pagesFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public PagesV3Controller(IPagesFlows pages)
        {
            this.pagesFlows = pages;
        }

        /// <summary>
        /// Return all active pages (pageSets and pageButtons are NOT included) for a specific POS for the current date ordered by Pages.Sort
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posid"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPagesForPosId/storeid/{storeid}/posid/{posid}")]
        public HttpResponseMessage GetPagesForPosId(string storeid, long posid)
        {
            PagesModelsPreview results = pagesFlows.GetPagesForPosId(DBInfo, storeid, posid);
            return Request.CreateResponse(HttpStatusCode.OK, results.PagesModel); // return results
        }

        /// <summary>
        /// Upsert a list of Pages from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpsertPages")]
        [System.Web.Http.Authorize]
        public HttpResponseMessage UpsertPages(List<PagesSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = pagesFlows.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}