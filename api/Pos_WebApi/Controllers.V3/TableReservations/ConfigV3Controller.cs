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
    [RoutePrefix("api/v3/Config")]
    public class ConfigV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IConfigFlows ConfigFlow;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public ConfigV3Controller(IConfigFlows conFlows)
        {
            this.ConfigFlow = conFlows;
        }


        /// <summary>
        /// Gets the Config
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("Get")]
        public HttpResponseMessage GetRestaurants()
        {
            ConfigModel result = ConfigFlow.GetConfig(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Insert new Config
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Insert")]
        [ValidateModelState]
        public HttpResponseMessage insertConfig(ConfigModel model)
        {
            long id = ConfigFlow.insertConfig(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        /// <summary>
        /// Update a specific Config to DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [ValidateModelState]
        public HttpResponseMessage UpdateConfig(ConfigModel Model)
        {
            ConfigModel result = ConfigFlow.UpdateConfig(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a specific Config from DB
        /// </summary>
        /// <param name="Id">ConfigID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete/Id/{Id}")]
        public HttpResponseMessage DeleteConfig(long Id)
        {
            long result = ConfigFlow.DeleteConfig(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}