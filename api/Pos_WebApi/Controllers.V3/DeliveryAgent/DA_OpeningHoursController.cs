using Pos_WebApi.Modules;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{
    //[RoutePrefix("api/v3/da/Loyalty")]
    public class DA_OpeningHoursController : BasicV3Controller
    {
        IDA_OpeningHoursFlows openinghoursFlow;

        public DA_OpeningHoursController(IDA_OpeningHoursFlows _openingshoursFlow)
        {
            this.openinghoursFlow = _openingshoursFlow;
        }

        /// <summary>
        /// Get DA Opening Hours
        /// </summary>
        /// <returns>Επιστρέφει τα περιεχόμενα των πινάκων DA_OpeningHours</returns>
        [HttpGet, Route("api/v3/da/OpeningHours/GetHours")]
        [Route("api/v3/OpeningHours/GetHours")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetHours()
        {
            List<DA_OpeningHoursModel> res = openinghoursFlow.GetHours(DBInfo);
              return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Save DA Opening Hours for Store
        /// </summary>
        /// <returns>Επιστρέφει τα περιεχόμενα των πινάκων DA_OpeningHours</returns>
        [HttpPost, Route("api/v3/da/OpeningHours/SaveForStore")]
        [Route("api/v3/OpeningHours/SaveForStore")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage SaveForStore(List<DA_OpeningHoursModel> hourslist)
        {
            openinghoursFlow.SaveForStore(DBInfo, hourslist);
            return Request.CreateResponse(HttpStatusCode.OK);
        }



        /// <summary>
        /// Save DA Opening Hours for ALL Stores
        /// </summary>
        /// <returns>Επιστρέφει τα περιεχόμενα των πινάκων DA_OpeningHours</returns>
        [HttpPost, Route("api/v3/da/OpeningHours/SaveForAllStores")]
        [Route("api/v3/OpeningHours/SaveForAllStores")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage SaveForAllStores(List<DA_OpeningHoursModel> hourslist)
        {
            // hourslist res = openinghoursFlow.GetHours(DBInfo);
            openinghoursFlow.SaveForAllStores(DBInfo, hourslist);
            return Request.CreateResponse(HttpStatusCode.OK);
        }



        [HttpPost, Route("api/v3/da/OpeningHours/CheckDA_OpeningHours")]
        [Route("api/v3/OpeningHours/CheckDA_OpeningHours")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage CheckDA_OpeningHours(dateHelper model)
        {
            // if (MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_OpeningHours") == false)
            //   return Request.CreateResponse(HttpStatusCode.OK,true);

            model = new dateHelper();
            model.estDate = Convert.ToDateTime("2020-11-27 12:15:53.000");
            //model.estDate = Convert.ToDateTime("2020-11-27 12:00:53.000");
            model.storeId = 2;
        bool res = openinghoursFlow.CheckDA_OpeningHours(DBInfo, model.estDate, model.storeId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

    public class dateHelper
    {
        public DateTime estDate { get; set; }
        public long storeId { get; set; }
    }

    }
}