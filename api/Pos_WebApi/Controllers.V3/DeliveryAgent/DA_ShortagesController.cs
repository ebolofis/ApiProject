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
    [RoutePrefix("api/v3/da/Shortages")]
    public class DA_ShortagesController : BasicV3Controller
    {
        IDA_ShortagesFlows shortagesFlow;

        public DA_ShortagesController(IDA_ShortagesFlows _shortagesFlow)
        {
            this.shortagesFlow = _shortagesFlow;
        }
      
        /// <summary>
        /// Get the List of Shortages 
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetList")]
        public HttpResponseMessage GetShortages()
        {
            List<DA_ShortagesExtModel> res = shortagesFlow.GetShortages(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Get a Shortage by id
        /// </summary>
        ///  <param name = "Id" > DA_ShortageProds.Id </ param >
        /// <returns></returns>
        [HttpGet, Route("shortage/Id/{id}")]
        public HttpResponseMessage GetShortage(int id)
        {
            DA_ShortagesExtModel res = shortagesFlow.GetShortage(DBInfo,id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Get a List of Shortages for a store based on staffId (a virtual staff assigned to a store). 
        /// Κλήση από το κατάστημα. 
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetShortagesByStore")]
        public HttpResponseMessage GetShortagesByStore()
        {
            List<DA_ShortagesExtModel> res = shortagesFlow.GetShortagesByStore(DBInfo, StaffId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Insert new Shortage 
        /// Κλήση από το κατάστημα ή τον agent. 
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("insert")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage Insert(DA_ShortageProdsModel model)
        {
            long res = shortagesFlow.Insert(DBInfo, model, StaffId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delete Shortage by Id
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("delete/Id/{id}")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage Delete(int Id)
        {
            long res = shortagesFlow.Delete(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delete all temporary Shortages for a store based on staffId (a virtual staff assigned to a store). 
        /// Κλήση από το κατάστημα. 
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("DeleteTemp")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage DeleteTemp()
        {
            long res = shortagesFlow.DeleteTemp(DBInfo, StaffId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Insert new Shortage 
        /// Κλήση από το κατάστημα ή τον agent. 
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("update")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage Update(DA_ShortageProdsModel model)
        {
            long res = shortagesFlow.Update(DBInfo, model, StaffId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}