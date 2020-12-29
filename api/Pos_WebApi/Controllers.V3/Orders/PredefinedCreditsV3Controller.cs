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
    [RoutePrefix("api/v3/PredefinedCredits")]
    public class PredefinedCreditsV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IPredefinedCreditsFlows predefinedCreditsFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public PredefinedCreditsV3Controller(IPredefinedCreditsFlows pages)
        {
            this.predefinedCreditsFlows = pages;
        }

        /// <summary>
        /// return all predefined credits (προκαθορισμένα  ποσά  για Ticket Restaurant/κουπόνια). 
        ///  GET api/PredefinedCredits
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns>list of PredefinedCredits</returns>
        [HttpGet, Route("GetPredefinedCredits/storeid/{storeid}")]
        public HttpResponseMessage GetPredefinedCredits(string storeid)
        {
            PredefinedCreditsModelsPreview results = predefinedCreditsFlows.GetPredefinedCredits(DBInfo, storeid);
            return Request.CreateResponse(HttpStatusCode.OK, results.PredefinedCreditsModel); // return results
        }
    }
}