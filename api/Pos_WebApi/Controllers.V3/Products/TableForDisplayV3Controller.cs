using Microsoft.AspNet.SignalR;
using Pos_WebApi.Models;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/TableForDisplay")]
    public class TableForDisplayV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        ITableForDisplayFlows tablefordisplay;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public TableForDisplayV3Controller(ITableForDisplayFlows tfd)
        {
            this.tablefordisplay = tfd;
        }

        /// <summary>
        /// Return table details
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="tableId"></param>
        /// <returns>
        [HttpGet, Route("GetSigleTable/storeid/{storeid}/tableId/{tableId}")]
        public HttpResponseMessage GetSigleTable(string storeid, long tableId)
        {
            try
            {
                TableForDisplayPreviewModel results = tablefordisplay.getSigleTable(DBInfo, storeid, tableId);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }

        /// <summary>
        /// Return Kitchen Instructions
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="tableId"></param>
        /// <returns>
        [HttpGet, Route("KitchenInstructions/storeid/{storeid}/tableId/{tableId}")]
        public HttpResponseMessage GetKitchenInstructions(string storeid, long tableId)
        {
            KitchenInstructionPreviewModel results = tablefordisplay.GetKitchenInstructionsForTable(DBInfo, storeid, tableId);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Return Tables Per Region
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="tableId"></param>
        /// <returns>
        [HttpGet, Route("GetOpenTablesPerRegion/storeid/{storeid}/regionId/{regionId}")]
        public HttpResponseMessage GetOpenTablesPerRegionStatusOnly(string storeid, long regionId)
        {
            TablesPerRegionPreviewModel results = tablefordisplay.GetOpenTablesPerRegionStatusOnly(DBInfo, storeid, regionId);
            return Request.CreateResponse(HttpStatusCode.OK, results); // return results
        }

        /// <summary>
        /// Get all tables for a specific pos ordered by Region and code (Όταν επιλέγει ο client Ενέργειες -> Ανανέωση Τραπεζιών)
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="posInfoId"></param>
        /// <returns>
        [HttpGet, Route("GetAllTables/storeid/{storeid}/posInfoId/{posInfoId}")]
        public HttpResponseMessage GetAllTables(string storeid, long posInfoId)
        {
            GetAllTablesModelPreview results = tablefordisplay.GetAllTables(DBInfo, storeid, posInfoId);
            return Request.CreateResponse(HttpStatusCode.OK, results.GetAllTable); // return results
        }
    }
}