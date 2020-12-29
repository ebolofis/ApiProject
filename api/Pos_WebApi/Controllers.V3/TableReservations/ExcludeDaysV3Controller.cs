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
    [RoutePrefix("api/v3/ExcludeDays")]
    public class ExcludeDaysV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IExcludeDaysFlows ExcludeDaysFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public ExcludeDaysV3Controller(IExcludeDaysFlows exDaysFlows)
        {
            this.ExcludeDaysFlows = exDaysFlows;
        }

        /// <summary>
        /// Gets the List of ExcludeDays
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetList")]
        public HttpResponseMessage GetExcludeDays()
        {
            ExcludeDaysListModel result = ExcludeDaysFlows.GetExcludeDays(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result.ExcludeDaysModelList);
        }

        /// <summary>
        /// Gets a specific ExcludeDay by Id
        /// </summary>
        /// <param name="Id">ExcludeDayID</param>
        /// <returns></returns>
        [HttpGet, Route("Get/Id/{Id}")]
        public HttpResponseMessage GetExcludeDayById(long Id)
        {
            ExcludeDaysModel result = ExcludeDaysFlows.GetExcludeDayById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Insert new ExcludeDay
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Insert")]
        [ValidateModelState]
        public HttpResponseMessage InsertExcludeDay(ExcludeDaysModel model)
        {
            long id = ExcludeDaysFlows.insertExcludeDay(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        /// <summary>
        /// Update a specific ExcludeDay to DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [ValidateModelState]
        public HttpResponseMessage UpdateExcludeDay(ExcludeDaysModel Model)
        {
            ExcludeDaysModel result = ExcludeDaysFlows.UpdateExcludeDay(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a specific ExcludeDay from DB
        /// </summary>
        /// <param name="Id">ExcludeDayID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete/Id/{Id}")]
        public HttpResponseMessage DeleteExcludeDay(long Id)
        {
            long result = ExcludeDaysFlows.DeleteExcludeDay(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Deleting old Exclude Days from DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("DeleteOld")]
        public HttpResponseMessage DeleteOldExcludeDays()
        {
            bool result = ExcludeDaysFlows.DeleteOldExcludeDays(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

    }
}