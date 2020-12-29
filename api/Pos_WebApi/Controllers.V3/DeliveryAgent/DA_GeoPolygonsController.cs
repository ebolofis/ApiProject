using Pos_WebApi.Modules;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{
   // [RoutePrefix("api/v3/da/Polygons")]
    public class DA_GeoPolygonsController : BasicV3Controller
    {
        IDA_GeoPolygonsFlows polygonsFlow;

        public DA_GeoPolygonsController(IDA_GeoPolygonsFlows _polygonsFlow)
        {
            this.polygonsFlow = _polygonsFlow;
        }

        /// <summary>
        /// Επιλογή καταστήματος (πολυγώνου) βάση AddressId
        /// </summary>
        /// <param name="Id">AddressId</param>
        /// <returns>StoreId (Εάν StoreId = 0 Τότε το κατάστημα δεν ανήκει στο πολύγωνο)</returns>
        [HttpGet, Route("api/v3/da/Polygons/Address/Id/{Id}")]
        [Route("api/v3/Polygons/Address/Id/{Id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage SelectPolygonByAddressId(long Id)
        {
            long res = polygonsFlow.SelectPolygonByAddressId(DBInfo, Id).StoreId ?? 0;
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Επιλογή καταστήματος (πολυγώνου) βάση Address Model
        /// </summary>
        /// <param name="Model">Address Model</param>
        /// <returns>StoreId (Εάν StoreId = 0 Τότε το κατάστημα δεν ανήκει στο πολύγωνο)</returns>
        [HttpPost, Route("api/v3/da/Polygons/Address")]
        [Route("api/v3/Polygons/Address")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage SelectPolygonByAddressModel (DA_AddressModel Model)
        {
            long res = polygonsFlow.SelectPolygonByAddressModel(DBInfo, Model).StoreId ?? 0;
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get a List of Polygons (Header/details). 
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Polygons/GetList")]
        [Route("api/v3/Polygons/GetList")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetPolygonsList()
        {
            List<DA_GeoPolygonsModel> res = polygonsFlow.GetPolygonsList(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get a List of Polygons (Header/details) by StoreId. 
        /// </summary>
        /// /// <param name="StoreId"></param>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Polygons/GetPolygons/StoreId/{StoreId}")]
        [Route("api/v3/Polygons/GetPolygons/StoreId/{StoreId}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetPolygonsByStore(long StoreId)
        {
            List<DA_GeoPolygonsModel> res = polygonsFlow.GetPolygonsByStore(DBInfo, StoreId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get a Polygon (Header/details) by Id. 
        /// </summary>
        /// /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Polygons/GetPolygon/Id/{Id}")]
        [Route("api/v3/Polygons/GetPolygon/Id/{Id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetPolygonById(long Id)
        {
            DA_GeoPolygonsModel res = polygonsFlow.GetPolygonById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Insert new Polygon (Header/details). 
        /// </summary>
        /// /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Polygons/Insert")]
        [Route("api/v3/Polygons/Insert")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage InsertPolygon(DA_GeoPolygonsModel Model)
        {
            long res = polygonsFlow.InsertPolygon(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update Polygon (Header/details). 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("api/v3/da/Polygons/Update")]
        [Route("api/v3/Polygons/Update")]
        public HttpResponseMessage UpdatePolygon(DA_GeoPolygonsModel Model)
        {
            long res = polygonsFlow.UpdatePolygon(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update DA_GeoPolygons Set Notes to NUll and IsActive = true
        /// <param name="StaffId"></param>
        /// </summary>
        [HttpPost, Route("api/v3/da/Polygons/UpdateDaPolygonsNotesActives")]
        [Route("api/v3/Polygons/UpdateDaPolygonsNotesActives")]
        public HttpResponseMessage UpdateDaPolygonsNotesActives()
        {
            long results = polygonsFlow.UpdateDaPolygonsNotesActives(DBInfo, StaffId);
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        /// <summary>
        /// Update Polygon's IsActive (true or false) by Id . 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Polygons/UpdateStatus/Id/{Id}/IsActive/{IsActive}")]
        [Route("api/v3/Polygons/UpdateStatus/Id/{Id}/IsActive/{IsActive}")]
        public HttpResponseMessage UpdatePolygonStatus(long Id, bool IsActive)
        {
            long res = polygonsFlow.UpdatePolygonStatus(DBInfo, Id, IsActive);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delete Polygon (Header/details). 
        /// </summary>
        /// <param name="Id">Header Id</param>
        /// <returns></returns>
        [HttpGet, Route("api/v3/da/Polygons/Delete/Id/{Id}")]
        [Route("api/v3/Polygons/Delete/Id/{Id}")]
        public HttpResponseMessage DeletePolygon(long Id)
        {
            long res = polygonsFlow.DeletePolygon(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Generate geometry shape value for each store polygon based on polygon details
        /// </summary>
        [HttpPost, Route("api/v3/da/Polygons/GenerateShapes")]
        public HttpResponseMessage GenerateShapes()
        {
            List<string> errors = polygonsFlow.GenerateShapes(DBInfo);
            
            return Request.CreateResponse(HttpStatusCode.OK, errors);
        }

        /// <summary>
        /// Insert records in DA_GeopolygonDetails generated from DA_Geopolygons.Shape string
        /// </summary>
        [HttpPost, Route("api/v3/da/Polygons/GenerateGeoPolygonDetailsFromShapes")]
        public HttpResponseMessage GenerateGeoPolygonDetailsFromShapes()
        {
            int res = polygonsFlow.GenerateGeoPolygonDetailsFromShapes(DBInfo);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}