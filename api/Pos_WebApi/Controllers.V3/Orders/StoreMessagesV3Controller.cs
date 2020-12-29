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
    [RoutePrefix("api/v3/StoreMessages")]
    public class StoreMessagesV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IStoreMessagesFlows storeMessagesFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public StoreMessagesV3Controller(IStoreMessagesFlows sm)
        {
            this.storeMessagesFlows = sm;
        }

        /// <summary>
        ///  Επιστρέφει τα μηνύματα που εμφανίζονται στην κύρια σελίδα
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="filtered"></param>
        /// <returns>
        [HttpGet, Route("GetStoreMessages/storeid/{storeid}/filtered/{filtered}")]
        public HttpResponseMessage GetStoreMessages(string storeid, bool filtered)
        {
            try
            {
                StoreMessagesModelsPreview results = storeMessagesFlows.GetStoreMessages(DBInfo, storeid, filtered);
                return Request.CreateResponse(HttpStatusCode.OK, results.storeMessages); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }

    }
}