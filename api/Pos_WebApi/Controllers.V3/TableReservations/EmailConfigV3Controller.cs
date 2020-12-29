using Microsoft.AspNet.SignalR;
using Pos_WebApi.Modules;
using Symposium.Models.Models.TableReservations;
using Symposium.WebApi.MainLogic.Interfaces.Flows.TableReservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/EmailConfig")]
    public class EmailConfigV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IEmailConfigFlows EmailConfigFlow;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public EmailConfigV3Controller(IEmailConfigFlows emailConFlows)
        {
            this.EmailConfigFlow = emailConFlows;
        }

        /// <summary>
        /// Gets the Email Config
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("Get")]
        public HttpResponseMessage GetEmailConfig()
        {
            EmailConfigModel result = EmailConfigFlow.GetEmailConfig(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Insert new Email Config
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Insert")]
        [ValidateModelState]
        public HttpResponseMessage insertConfig(EmailConfigModel model)
        {
            EmailConfigModel res = EmailConfigFlow.InsertEmailConfig(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update a specific Email Config to DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [ValidateModelState]
        public HttpResponseMessage UpdateEmailConfig(EmailConfigModel Model)
        {
            EmailConfigModel result = EmailConfigFlow.UpdateEmailConfig(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a specific Email Config from DB
        /// </summary>
        /// <param name="Id">EmailConfigID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete/Id/{Id}")]
        public HttpResponseMessage DeleteEmailConfig(long Id)
        {
            long result = EmailConfigFlow.DeleteEmailConfig(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}