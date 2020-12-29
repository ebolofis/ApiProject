using Symposium.WebApi.Controllers.V3;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Kds;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Symposium.Models.Models.Kds;
using Symposium.Helpers;

namespace Pos_WebApi.Controllers.V3.KDS
{
    [RoutePrefix("api/v3/Kds")]
    public class KdsV3Controller : BasicV3Controller
    {
        IKdsFlows kdsflows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();
        public KdsV3Controller(IKdsFlows _kdsflows)
        {
            this.kdsflows = _kdsflows;
        }

        /// <summary>
        /// Get All Order Details In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>KdsResponceModel</returns>
        [HttpPost, Route("GetKdsOrders")]
        public HttpResponseMessage GetKdsOrders(KdsGetOrdersRequestModel Model)
        {
            List<KdsResponceModel> res = kdsflows.GetKdsOrders(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update Kitchen Status for Specific Order Detail
        /// </summary>
        /// <returns>KdsUpdateResponceModel</returns>
        [HttpPost, Route("UpdateKitchenStatus")]
        public HttpResponseMessage UpdateKitchenStatus(KdsUpdateKitchenStatusRequestModel Model)
        {
            KdsUpdateResponceModel res = kdsflows.UpdateKitchenStatus(DBInfo, Model);
            //Send SignalR Signal to Update Kds UI
            hub.Clients.Group(Model.Storeid).kdsSignalToUpdateUI();
            //Send SignalR Signal to Dispatcher
            if (res.SendSignalR == true)
            {
                List<long> KdsOrdersIdList = new List<long>();
                KdsOrdersIdList = KdsOrderIdListHelper.addKdsOrderIdsToList(Model.OrderId);
                hub.Clients.Group(Model.Storeid).kdsMessageToDispatcher(Model.Storeid, KdsOrdersIdList);
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Delete one Ingredient and update PendingQty
        /// </summary>
        /// <returns>List KdsResponceModel</returns>
        [HttpPost, Route("DeleteIngredient")]
        public HttpResponseMessage DeleteIngredient(KdsDeleteIngredientsRequestModel Model)
        {
            List<KdsResponceModel> res = kdsflows.DeleteIngredient(DBInfo, Model);
            //Send SignalR Signal to Update UI
            hub.Clients.Group(Model.Storeid).kdsSignalToUpdateUI();
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get All Ingredients In Status Kitchen for Specific Kds and SalesTypes
        /// </summary>
        /// <returns>List OrderDetailIngredientsModel</returns>
        [HttpPost, Route("GetKdsIngredients")]
        public HttpResponseMessage GetKdsIngredients(KdsGetOrdersRequestModel Model)
        {
            List<KdsIngredientsResponceModel> res = kdsflows.GetKdsIngredients(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}