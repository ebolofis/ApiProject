using Microsoft.AspNet.SignalR;
using Symposium.Models.Models;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/DeliveryOrders")]

    public class DeliveryOrdersV3Controller : BasicV3Controller
    {
        IDeliveryOrdersFlows doflows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Pos_WebApi.Hubs.WebPosHub>();

        public DeliveryOrdersV3Controller(IDeliveryOrdersFlows _doflows)
        {
            this.doflows = _doflows;
        }
        [HttpGet, Route("test")]
        public HttpResponseMessage test()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet, Route("GetKdsOrdersIdList")]
        public HttpResponseMessage GetKdsOrdersIdList()
        {
            List<long> kdsOrdersId = doflows.GetKdsOrdersIdList(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, kdsOrdersId);
        }

        [HttpGet, Route("ClearKdsOrdersIdList")]
        public HttpResponseMessage ClearKdsOrdersIdList()
        {
            bool res = doflows.ClearKdsOrdersIdList(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("StateCounts")]
        public HttpResponseMessage StateCounts(DeliveryFilters Model)
        {
            var res = doflows.StateCounts(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
            //return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost, Route("GetOrderStatusTimeChanges/{OrderId}")]
        public HttpResponseMessage GetOrderStatusTimeChanges(long OrderId)
        {
            var res = doflows.GetOrderStatusTimeChanges(DBInfo, OrderId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
        [HttpPost, Route("PagedOrdersByState/{status}/{pageNumber}/{pageLength}")]
        public HttpResponseMessage PagedOrdersByState(int status, int pageNumber, int pageLength, DeliveryFilters Model)
        {
            PaginationModel<DeliveryStatusOrders> res = doflows.PagedOrdersByStateFlow(DBInfo, status, pageNumber, pageLength, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        [HttpPost, Route("UpdateShippingCoordinates/{InvoiceId}")]
        public HttpResponseMessage UpdateShippingCoordinates(long InvoiceId, DeliveryCustomersShippingAddressModel Model)
        {
            var res = doflows.UpdateShippingCoordinates(DBInfo, InvoiceId, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("UpdateOrderStatusForReturned/{OrderId}/{status}")]
        public HttpResponseMessage UpdateOrderStatusForReturned(long OrderId, int status)
        {
            int res = doflows.UpdateOrderStatusForReturned(DBInfo, OrderId, status);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("UpdateStaffStatus/{StaffId}/{IsOnRoad}")]
        public HttpResponseMessage UpdateStaffStatus(long StaffId, bool IsOnRoad)
        {
            long res = doflows.UpdateStaffStatus(DBInfo, StaffId, IsOnRoad);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("GetDeliveryStaffsOnly")]
        public HttpResponseMessage GetDeliveryStaffsOnly()
        {
            List<StaffDeliveryModel> res = doflows.GetDeliveryStaffsOnly(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("GetDeliveryStaffsSignInOnly/PosInfoId/{PosInfoId}")]
        public HttpResponseMessage GetDeliveryStaffsSignInOnly(long PosInfoId)
        {
            List<StaffDeliveryModel> res = doflows.GetDeliveryStaffsSignInOnly(DBInfo, PosInfoId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

    }
}