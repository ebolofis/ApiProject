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
    [RoutePrefix("api/v3/Restrictions")]
    public class RestrictionsV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IRestrictionFlows RestrictionsFlow;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public RestrictionsV3Controller(IRestrictionFlows restFlows)
        {
            this.RestrictionsFlow = restFlows;
        }

        /// <summary>
        /// Gets the List of Restrictions
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetList")]
        public HttpResponseMessage GetRestrictions()
        {
            RestrictionsListModel result = RestrictionsFlow.GetRestrictions(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result.RestrictionsModelList);
        }

        /// <summary>
        /// Gets a specific Restriction by Id
        /// </summary>
        /// <param name="Id">RestrictionID</param>
        /// <returns></returns>
        [HttpGet, Route("Get/Id/{Id}")]
        public HttpResponseMessage GetRestrictionById(long Id)
        {
            RestrictionsModel result = RestrictionsFlow.GetRestrictionById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Insert new Restriction
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Insert")]
        [ValidateModelState]
        public HttpResponseMessage insertRestriction(RestrictionsModel model)
        {
            long id = RestrictionsFlow.insertRestriction(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        /// <summary>
        /// Update a specific Restriction to DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [ValidateModelState]
        public HttpResponseMessage UpdateRestriction(RestrictionsModel Model)
        {
            RestrictionsModel result = RestrictionsFlow.UpdateRestriction(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a specific Restriction from DB
        /// </summary>
        /// <param name="Id">RestrictionID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete/Id/{Id}")]
        public HttpResponseMessage DeleteRestriction(long Id)
        {
            long result = RestrictionsFlow.DeleteRestriction(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}