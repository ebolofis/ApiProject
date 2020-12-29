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
    [RoutePrefix("api/v3/Capacities")]
    public class CapacitiesV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        ICapacitiesFlows CapacitiesFlow;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public CapacitiesV3Controller(ICapacitiesFlows capFlows)
        {
            this.CapacitiesFlow = capFlows;
        }

        /// <summary>
        /// Gets the List of Capacities
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetList")]
        public HttpResponseMessage GetCapacities()
        {
            CapacitiesListModel result = CapacitiesFlow.GetCapacities(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result.CapacitiesModelList);
        }

        /// <summary>
        /// Gets the List of Capacities
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetList/RestId/{RestId}")]
        public HttpResponseMessage GetCapacitiesByRestaurant(long RestId)
        {
            CapacitiesListModel result = CapacitiesFlow.GetCapacitiesByRestId(DBInfo, RestId);
            return Request.CreateResponse(HttpStatusCode.OK, result.CapacitiesModelList);
        }

        /// <summary>
        /// Gets a specific Capacity by Id
        /// </summary>
        /// <param name="Id">CapacityID</param>
        /// <returns></returns>
        [HttpGet, Route("Get/Id/{Id}")]
        public HttpResponseMessage GetCapacityById(long Id)
        {
            CapacitiesModel result = CapacitiesFlow.GetCapacityById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Insert new Capacity
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Insert")]
        [ValidateModelState]
        public HttpResponseMessage InsertCapacity(CapacitiesModel model)
        {
            long id = CapacitiesFlow.insertCapacity(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        /// <summary>
        /// Update a specific Capacity to DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [ValidateModelState]
        public HttpResponseMessage UpdateCapacity(CapacitiesModel Model)
        {
            CapacitiesModel result = CapacitiesFlow.UpdateCapacity(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delete a specific Capacity from DB
        /// </summary>
        /// <param name="Id">RestaurantID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete/Id/{Id}")]
        public HttpResponseMessage DeleteCapacity(long Id)
        {
            long result = CapacitiesFlow.DeleteCapacity(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}