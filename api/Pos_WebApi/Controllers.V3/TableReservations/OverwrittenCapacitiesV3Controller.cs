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
    [RoutePrefix("api/v3/OverwrittenCapacities")]
    public class OverwrittenCapacitiesV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IOverwrittenCapacitiesFlows OverwrittenCapacitiesFlow;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public OverwrittenCapacitiesV3Controller(IOverwrittenCapacitiesFlows overFlows)
        {
            this.OverwrittenCapacitiesFlow = overFlows;
        }

        /// <summary>
        /// Gets the List of Overwritten Capacities
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetList")]
        public HttpResponseMessage GetOverwrittenCapacities()
        {
            OverwrittenCapacitiesListModel result = OverwrittenCapacitiesFlow.GetOverwrittenCapacities(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result.OverwrittenCapacitiesModelList);
        }

        /// <summary>
        /// Gets a specific Overwritten Capacity by Id
        /// </summary>
        /// <param name="Id">OverwrittenCapacityID</param>
        /// <returns></returns>
        [HttpGet, Route("Get/Id/{Id}")]
        public HttpResponseMessage GetOverwrittenCapacityById(long Id)
        {
            OverwrittenCapacitiesModel result = OverwrittenCapacitiesFlow.GetOverwrittenCapacityById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Insert new OverwrittenCapacity
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Insert")]
        [ValidateModelState]
        public HttpResponseMessage insertOverwrittenCapacity(OverwrittenCapacitiesModel model)
        {
            long id = OverwrittenCapacitiesFlow.insertOverwrittenCapacity(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        /// <summary>
        /// Update a specific OverwrittenCapacity to DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [ValidateModelState]
        public HttpResponseMessage UpdateOverwrittenCapacity(OverwrittenCapacitiesModel Model)
        {
            OverwrittenCapacitiesModel result = OverwrittenCapacitiesFlow.UpdateOverwrittenCapacity(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a specific OverwrittenCapacity from DB
        /// </summary>
        /// <param name="Id">OverwrittenCapacityID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete/Id/{Id}")]
        public HttpResponseMessage DeleteOverwrittenCapacity(long Id)
        {
            long result = OverwrittenCapacitiesFlow.DeleteOverwrittenCapacity(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

    }
}