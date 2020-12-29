using Microsoft.AspNet.SignalR;
using Pos_WebApi;
using Pos_WebApi.Models;
using Symposium_DTOs.PosModel_Info;
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
using Symposium.Models.Models.Products;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/PageButton")]
    public class PageButtonV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IPageButtonFlows pageButtonFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public PageButtonV3Controller(IPageButtonFlows pb)
        {
            this.pageButtonFlows = pb;
        }

        /// <summary>
        /// Return page buttons for a specific Pos, PageId and Pricelist
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posid"></param>
        /// <param name="pageid"></param>
        /// <param name="pricelistid"></param>
        /// <returns></returns>
        [HttpGet, Route("GetPageButtons/storeid/{storeid}/posid/{posid}/pageid/{pageid}/pricelistid/{pricelistid}/isPos/{isPos}")]
        public HttpResponseMessage GetPageButtons(string storeid, int posid, int pageid, int pricelistid, bool isPos = false)
        {
            PageButtonPreviewModel results = pageButtonFlows.GetPageButtons(DBInfo, storeid, posid, pageid, pricelistid, isPos);
            return Request.CreateResponse(HttpStatusCode.OK, results.PageButtons); // return results
        }


       

        /// <summary>
        /// Upsert a list of Page Button from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpsertPageButton")]
        [System.Web.Http.Authorize]
        public HttpResponseMessage UpsertPageButton(List<PageButtonSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = pageButtonFlows.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <returns></returns>
        [HttpPost, Route("SearchExternalProduct/{description}")]
        public HttpResponseMessage SearchExternalProduct(string description)
        {
           List<PageButtonPricelistDetailsAssoc> res = pageButtonFlows.SearchExternalProduct(DBInfo, description);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}