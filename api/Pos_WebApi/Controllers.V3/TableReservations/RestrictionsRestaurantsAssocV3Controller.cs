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
    [RoutePrefix("api/v3/RestrictionsRestaurantsAssoc")]
    public class RestrictionsRestaurantsAssocV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        IRestrictionsRestaurantsAssocFlows RestrictionsRestaurantsAssocFlow;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public RestrictionsRestaurantsAssocV3Controller(IRestrictionsRestaurantsAssocFlows rraFlows)
        {
            this.RestrictionsRestaurantsAssocFlow = rraFlows;
        }

        /// <summary>
        /// Gets the List of Restrictions Restaurants Associations
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetList")]
        public HttpResponseMessage GetRestrictionsRestaurantsAssoc()
        {
            RestrictionsRestaurantsAssocListModel result = RestrictionsRestaurantsAssocFlow.GetRestrictionsRestaurantsAssoc(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, result.RestrictionsRestaurantsAssocList);
        }

        /// <summary>
        /// Gets a specific Restrictions_Restaurants_Assoc by Id
        /// </summary>
        /// <param name="Id">RestrictionsRestaurantsAssocID</param>
        /// <returns></returns>
        [HttpGet, Route("Get/Id/{Id}")]
        public HttpResponseMessage GetRestrictionsRestaurantsAssocId(long Id)
        {
            RestrictionsRestaurantsAssocModel result = RestrictionsRestaurantsAssocFlow.GetRestrictionsRestaurantsAssocById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        /// <summary>
        /// Insert new Restrictions_Restaurants_Assoc
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Insert")]
        [ValidateModelState]
        public HttpResponseMessage InsertRestrictionsRestaurantsAssoc(RestrictionsRestaurantsAssocModel model)
        {
            long id = RestrictionsRestaurantsAssocFlow.insertRestrictionsRestaurantsAssoc(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        /// <summary>
        /// Update a specific Restrictions_Restaurants_Assoc to DB
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [ValidateModelState]
        public HttpResponseMessage UpdateRestrictionsRestaurantsAssoc(RestrictionsRestaurantsAssocModel Model)
        {
            RestrictionsRestaurantsAssocModel result = RestrictionsRestaurantsAssocFlow.UpdateRestrictionsRestaurantsAssoc(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// Delets a specific Restrictions_Restaurants_Assoc from DB
        /// </summary>
        /// <param name="Id">RestrictionsRestaurantsAssocID</param>
        /// <returns></returns>
        [HttpPost, Route("Delete/Id/{Id}")]
        public HttpResponseMessage DeleteRestrictionsRestaurantsAssoc(long Id)
        {
            long result = RestrictionsRestaurantsAssocFlow.DeleteRestrictionsRestaurantsAssoc(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}