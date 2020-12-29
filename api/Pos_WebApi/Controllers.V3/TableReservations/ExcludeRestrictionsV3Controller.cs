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
    [RoutePrefix("api/v3/ExcludeRestrictions")]
    public class ExcludeRestrictionsV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IExcludeRestrictionsFlows excludeRestrictionsFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public ExcludeRestrictionsV3Controller(IExcludeRestrictionsFlows exRestrictionsFlows)
        {
            this.excludeRestrictionsFlows = exRestrictionsFlows;
        }

        /// <summary>
        /// Gets the List of ExcludeRestrictions
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetList")]
        public HttpResponseMessage GetExcludeRestrictions()
        {
            ExcludeRestrictionsListModel result = excludeRestrictionsFlows.GetExcludeRestrictions(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result.ExcludeRestrictionsList);
        }

        /// <summary>
        /// Gets a specific ExcludeRestriction by Id
        /// </summary>
        /// <param name="Id">ExcludeRestrictionID</param>
        /// <returns></returns>
        [HttpGet, Route("Get/Id/{Id}")]
        public HttpResponseMessage GetExcludeRestrictionById(long Id)
        {
            ExcludeRestrictionsModel result = excludeRestrictionsFlows.GetExcludeRestrictionById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        //TO DO: ComboList

        /// <summary>
        /// Insert new ExcludeRestriction
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Insert")]
        [ValidateModelState]
        public HttpResponseMessage InsertExcludeRestriction(ExcludeRestrictionsModel model)
        {
            long id = excludeRestrictionsFlows.insertExcludeRestriction(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        /// <summary>
        /// Update a specific ExcludeRestriction to DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [ValidateModelState]
        public HttpResponseMessage UpdateExcludeRestriction(ExcludeRestrictionsModel Model)
        {
            ExcludeRestrictionsModel result = excludeRestrictionsFlows.UpdateExcludeRestriction(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a specific ExcludeRestriction from DB
        /// </summary>
        /// <param name="Id">ExcludeRestrictionID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete/Id/{Id}")]
        public HttpResponseMessage DeleteExcludeRestriction(long Id)
        {
            long result = excludeRestrictionsFlows.DeleteExcludeRestriction(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Deleting old Exclude Restrictions from DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("DeleteOld")]
        public HttpResponseMessage DeleteOldExcludeRestrictions()
        {
            bool result = excludeRestrictionsFlows.DeleteOldExcludeRestrictions(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}