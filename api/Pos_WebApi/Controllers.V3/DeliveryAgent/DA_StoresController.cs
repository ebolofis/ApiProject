using Pos_WebApi.Modules;
using Symposium.Models.Enums;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{


    [RoutePrefix("api/v3/da/Stores")]
    public class DA_StoresController : BasicV3Controller
    {
        IDA_StoresFlows storesFlow;

        public DA_StoresController(IDA_StoresFlows _storesFlow)
        {
            this.storesFlow = _storesFlow;
        }

        [HttpGet, Route("GetBOStores")]

        public HttpResponseMessage GetBOStores()
        {
            List<DA_StoreModel> res = storesFlow.GetStores(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
        /// <summary>
        /// Get a List of Stores
        /// </summary>
        /// <returns>DA_StoreModel</returns>
        [HttpGet, Route("GetStores")]
        [Authorize]
        public HttpResponseMessage GetStores()
        {
            List<DA_StoreModel> res = storesFlow.GetStores(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get a List of Stores With Latitude and Longtitude
        /// </summary>
        /// <returns>DA_StoreInfoModel</returns>
        [HttpPost, Route("GetStoresPosition")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetStoresPosition()
        {
            List<DA_StoreInfoModel> res = storesFlow.GetStoresPosition(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get A Specific Order
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet, Route("GetStoreById/Id/{Id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetStoreById(long Id)
        {
            DA_StoreModel res = storesFlow.GetStoreById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get A Specific Store based on StaffId. 
        /// Require Header: STAFFID, StaffId id the staff's id in DA Server. 
        /// Κλήση από το κατάστημα μόνο.
        /// </summary>
        /// <param name="staffId">staff.Id</param>
        [HttpGet, Route("GetStoreBystaffId")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetStoreByStaffId()
        {
            DA_StoreInfoModel res = storesFlow.GetStoreByStaffId(DBInfo, StaffId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update DA_Store Set Notes to NUll
        /// <param name="StaffId"></param>
        /// </summary>
        [HttpPost, Route("UpdateDaStoreNotes")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage UpdateDaStoreNotes()
        {
            long results = storesFlow.UpdateDaStoreNotes(DBInfo, StaffId);
            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        /// <summary>
        /// insert a new store
        /// </summary>
        /// <param name="StoreModel">StoreModel</param>
        /// <returns>the new Id</returns>
        [HttpPost, Route("add")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage Insert(DA_StoreModel StoreModel)
        {
            long id = storesFlow.Insert(DBInfo, StoreModel);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        [HttpPost, Route("addBOStores")]

        public HttpResponseMessage addBOStores(DA_StoreModel StoreModel)
        {
            long id = storesFlow.Insert(DBInfo, StoreModel);
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        /// <summary>
        /// update a  store
        /// </summary>
        /// <param name="StoreModel">StoreModel</param>
        /// <returns>the records affected</returns>
        [HttpPost, Route("update")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage Update(DA_StoreModel StoreModel)
        {
            long count = storesFlow.Update(DBInfo, StoreModel);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }


        [HttpPost, Route("BOupdate")]

        public HttpResponseMessage BOUpdate(DA_StoreModel StoreModel)
        {
            long count = storesFlow.Update(DBInfo, StoreModel);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        /// <summary>
        /// update a  store
        /// </summary>
        /// <param name="StoreModel">StoreModel</param>
        /// <returns>the records affected</returns>
        [HttpGet, Route("delete/{id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage Delete(long Id)
        {
            long count = storesFlow.Delete(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }


        //BO Delete for Managing Stores - i.e Manage DA Stores Section
        [HttpPost, Route("BOdelete/Id/{Id}")]

        public HttpResponseMessage BODelete(long Id)
        {
            long count = storesFlow.BODelete(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }
        /// <summary>
        /// Update Store's DeliveryTime, TakeOutTime, StoreStatus  (Require Header: STAFFID, StaffId id the staff's id in DA Server). 
        /// Κλήση από το κατάστημα μόνο.
        /// </summary>
        /// <param name="d">deliveryTime (min)</param>
        /// <param name="t">takeOutTime (min)</param>
        /// <param name="s">storeStatus</param>
        [HttpGet, Route("update/delivery/{d}/takeout/{t}/status/{s}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage UpdateTimesStatus(int d,int t, DAStoreStatusEnum s)
        {
            storesFlow.UpdateTimesStatus(DBInfo, StaffId, d,t,s);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet, Route("updateStoreTables/StoreId/{storeId}")]
        [Authorize]
        [IsDA]
        public async Task<HttpResponseMessage> UpdateStoreTables(int StoreId)
        {
            
            Task.Run(() => storesFlow.UpdateClientStore(DBInfo, StoreId));
            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}