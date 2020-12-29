using Microsoft.AspNet.SignalR;
using Pos_WebApi.Modules;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{
    [RoutePrefix("api/v3/da/PhoneCenter")]
    public class DA_PhoneCenterController : BasicV3Controller
    {
        /// <summary>
        /// static dictionary that keeps all active calls from BarPhone
        /// </summary>
        public static Dictionary<string, BarPhoneModel> BarPhonerCalls = new Dictionary<string, BarPhoneModel>();

        /// <summary>
        /// id's prefix for SignalR  
        /// </summary>
        const string prefix = "Agent_";

        /// <summary>
        /// posted new phone call from BarPhone (phone center)
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("barphone")]
        [AllowAnonymous]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage PostBarPhone(BarPhoneModel model)
        {
            redirectCall(model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// posted new phone call from phone center
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("newcall")]
        [AllowAnonymous]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage NewCall(BarPhoneModel model)
        {
            redirectCall(model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }



        /// <summary>
        /// get the current customer phone for a specific agentId. Return customer phone or null
        /// </summary>
        /// <param name="agentId">agentId, ex: Agent_123</param>
        /// <returns></returns>
        [HttpGet, Route("getCustPhone/{agentId}")]
        [IsDA]
        public HttpResponseMessage GetCustPhone(string agentId)
        {
            if (!BarPhonerCalls.ContainsKey(agentId))
                return Request.CreateResponse<string>(HttpStatusCode.OK, null);
            else
                return Request.CreateResponse(HttpStatusCode.OK, BarPhonerCalls[agentId].CustPhone);
        }

        /// <summary>
        /// finish a customer call for a specific agentId.
        /// </summary>
        /// <param name="agentId">agentId, ex: Agent_123</param>
        /// <returns></returns>
        [HttpGet, Route("finishCall/{agentId}")]
        [IsDA]
        public HttpResponseMessage FinishCall(string agentId)
        {
            //remove call from dictionary
            lock (BarPhonerCalls)
            {
                BarPhonerCalls.Remove(agentId);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        /// <summary>
        /// redirect a phone call from  Telephone Center to Agent
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        private void redirectCall(BarPhoneModel model)
        {
            model.CreateTime = DateTime.Now;
            model.AgentId = prefix + model.AgentId;         // convert AgentId, ex: 123 --> Agent_123

            lock (BarPhonerCalls)
            {
                if (!BarPhonerCalls.ContainsKey(model.AgentId))
                    BarPhonerCalls.Add(model.AgentId, model); // add call to dictionary
                else
                    BarPhonerCalls[model.AgentId] = model;    // update call to dictionary
            }

            //send signalR message to agent
            string conId = HubParticipantsController.Participants.GetConnectionId(model.AgentId);
            if (conId != null)
            {
                GlobalHost.ConnectionManager.GetHubContext("DAHub").Clients.Client(conId).GetCustPhone(model.CustPhone);
                // or: IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<DA_Hub>();
            }
            else
                logger.Warn("SignalR User '" + model.AgentId + "' NOT FOUND in the list of SignalR clients (HubParticipantsController.Participants) !!!");

            logger.Info($"New Phone Call {model.CustPhone} for {model.AgentId}");
            removeOldItems();
        }

        /// <summary>
        /// remove old entries from dictionary
        /// </summary>
        private void removeOldItems()
        {
            var now = DateTime.Now;
            lock (BarPhonerCalls)
            {
                foreach (string key in BarPhonerCalls.Keys)
                {
                    var item = BarPhonerCalls[key];
                    if (item.CreateTime.AddMinutes(15) < now)
                        BarPhonerCalls.Remove(key);
                }
            }
        }

    }
}
