using Microsoft.AspNet.SignalR;
using Pos_WebApi.Models.ExtecrModels;
using Symposium.Helpers;
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
    [RoutePrefix("api/v3/ReceiptDetailsForExtecr")]
    public class ReceiptDetailsForExtecrV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IReceiptDetailsForExtecrFlows receiptDetailsForExtecrFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public ReceiptDetailsForExtecrV3Controller(IReceiptDetailsForExtecrFlows resFlows)
        {
            this.receiptDetailsForExtecrFlows = resFlows;
        }

        /// <summary>
        /// Get Receipt Details For Extecr
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>
        [HttpGet, Route("GetReceiptDetailsForExtecr/invoiceId/{invoiceId}")]
        public HttpResponseMessage GetReceiptDetailsForExtecr(Int64 invoiceId, bool isForKitchen = false)
        {
            logger.Info(" > Printing invoiceId: " + invoiceId.ToString() + ". ForKitchen:" + isForKitchen.ToString()+"...");
            bool groupReceiptItems = MainConfigurationHelper.GetSubConfiguration("api", "GroupReceiptItems");
            if (isForKitchen)
            {
                groupReceiptItems = false;
            }

            ExtecrReceiptModel results = receiptDetailsForExtecrFlows.GetReceiptDetailsForExtecr(DBInfo, invoiceId, groupReceiptItems, isForKitchen);
            var a = Request.CreateResponse(HttpStatusCode.OK, results); // return results
            return a;
        }
    }
}