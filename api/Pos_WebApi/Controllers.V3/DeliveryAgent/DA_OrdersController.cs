using Pos_WebApi.Modules;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.WebGoodysOrders;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{
    [RoutePrefix("api/v3/da/Orders")]
    public class DA_OrdersController : BasicV3Controller
    {
        IDA_OrdersFlows ordersFlow;
        IDA_OrderStatusFlows orderStatusFlow;

        public DA_OrdersController(IDA_OrdersFlows _ordersFlow, IDA_OrderStatusFlows orderStatusFlow)
        {
            this.ordersFlow = _ordersFlow;
            this.orderStatusFlow = orderStatusFlow;
        }

        /// <summary>
        /// Return the status of an Order
        /// </summary>
        /// <param name="id">Order id</param>
        /// <returns></returns>
        [HttpGet, Route("order/{id}/status")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetOrderStatus(long id)
        {
            OrderStatusEnum res = ordersFlow.GetStatus(DBInfo, id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get All Orders
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetAllOrders")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetAllOrders()
        {
            List<DA_OrderModelExt> res = ordersFlow.GetAllOrders(DBInfo);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get Orders ByDate
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetOrdersByDate/{SelectedDate}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetOrdersByDate(string SelectedDate)
        {
            List<DA_OrderModelExt> res = ordersFlow.GetOrdersByDate(DBInfo, SelectedDate);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get Customer Recent Orders
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="top"></param>
        /// <returns>Επιστρέφει τις τελευταίες παραγγελίες ενός πελάτη</returns>
        [HttpGet, Route("GetOrders/customer/{id}/top/{top}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetOrders(long id, int top)
        {
            List<DA_OrderModel> res = ordersFlow.GetOrders(DBInfo, id, top);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get Customer Recent historic Orders ( status: Canceled =5,Complete = 6, Returned=7)
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="top"></param>
        /// <returns>Επιστρέφει τις τελευταίες παραγγελίες ενός πελάτη</returns>
        [HttpGet, Route("GetHistoricOrders/customer/{id}/top/{top}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetHistoricOrders(long id, int top)
        {
            List<DA_OrderModel> res = ordersFlow.GetOrders(DBInfo, id, top, GetOrdersFilterEnum.Historic);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get Customer Recent NON Pending Orders ( status: 11)
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="top"></param>
        /// <returns>Επιστρέφει τις τελευταίες παραγγελίες ενός πελάτη</returns>
        [HttpGet, Route("GetNonPendingOrders/customer/{id}/top/{top}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetNonPendingOrders(long id, int top)
        {
            List<DA_OrderModel> res = ordersFlow.GetOrders(DBInfo, id, top, GetOrdersFilterEnum.NotPending);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get A Specific Order
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Order + details + ShippingAddress</returns>
        [HttpGet, Route("GetOrderById/Id/{Id}")]
        [Authorize]
        public HttpResponseMessage GetOrderById(long Id)
        {
            DA_ExtOrderModel res = ordersFlow.GetOrderById(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Search for Orders
        /// </summary>
        /// <param name="Model">Filter Model</param>
        /// <returns>Επιστρέφει τις παραγγελίες βάση κριτηρίων</returns>
        [HttpPost, Route("Search")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage SearchOrders(DA_SearchOrdersModel Model)
        {
            List<DA_OrderModel> res = ordersFlow.SearchOrders(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Mεταβάλλει το DA_orders. StatusChange και εισάγει νέα εγγραφή στον DA_OrderStatus
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        [HttpGet, Route("UpdateStatus/Id/{Id}/Status/{Status}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage UpdateStatus(long Id, OrderStatusEnum Status)
        {
            long res = ordersFlow.UpdateStatus(DBInfo, Id, Status);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        /// <summary>
        /// Get Customer Recent Remarks
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <param name="top"></param>
        /// <returns>Επιστρέφει τις τελευταίες παραγγελίες ενός πελάτη Remarks != null</returns>
        [HttpGet, Route("GetRemarks/customer/{id}/top/{top}")]
        [Authorize]
        public HttpResponseMessage GetRemarks(long Id, int top)
        {
            List<DA_OrderModel> res = ordersFlow.GetRemarks(DBInfo, Id, top);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Get Order Status For Specific Order by OrderId
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetOrderStatusTimeChanges/OrderId/{OrderId}")]
        [Authorize]
        public HttpResponseMessage GetOrderStatusTimeChanges(long OrderId)
        {
            List<DA_OrderStatusModel> res = ordersFlow.GetOrderStatusTimeChanges(DBInfo, OrderId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update Customer Remarks
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateRemarks")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage UpdateRemarks(UpdateRemarksModel Model)
        {
            long res = ordersFlow.UpdateRemarks(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Add new Order 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("Add")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage InsertOrder(DA_OrderModel Model)
        {
            long res = ordersFlow.InsertOrder(DBInfo, Model,CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update an Order (from DA or WEB only)
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("Update")]
        [Authorize]
        [ValidateModelState]
        [CheckModelForNull]
        [IsDA]
        public HttpResponseMessage UpdateOrder(DA_OrderModel Model)
        {
            long res = ordersFlow.UpdateOrder(DBInfo, Model,CustomerId);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Ακύρωση παραγγελίας από όλους εκτός από το κατάστημα. 
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <returns></returns>
        [HttpGet, Route("Cancel/Id/{Id}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage CancelOrder(long Id)
        {
            string daCancelStatusesStringRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_Cancel");
            string daCancelStatusesString = daCancelStatusesStringRaw.Trim();
            IEnumerable<OrderStatusEnum> cancelStasus = (IEnumerable<OrderStatusEnum>)daCancelStatusesString.Split(',').Select(s => Convert.ToInt32(s)).Cast<OrderStatusEnum>().ToArray();
            long res = ordersFlow.CancelOrder(DBInfo, Id, cancelStasus.ToArray());
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Ακύρωση παραγγελίας από το κατάστημα MONO.  
        /// </summary>
        /// <param name="Id">Order Id</param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        [HttpGet, Route("StoreCancel/Id/{Id}/StoreId/{StoreId}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage StoreCancelOrder(long Id, long StoreId)
        {
            string daCancelStatusesStringRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_Cancel");
            string daCancelStatusesString = daCancelStatusesStringRaw.Trim();
            IEnumerable<OrderStatusEnum> cancelStasus = (IEnumerable<OrderStatusEnum>)daCancelStatusesString.Split(',').Select(s => Convert.ToInt32(s)).Cast<OrderStatusEnum>().ToArray();
            long res = ordersFlow.StoreCancelOrder(DBInfo, Id, StoreId, cancelStasus.ToArray());
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Επιλογή Των Order Status που επιτρέπεται η ακύρωση Παραγγελίας.
        /// </summary>
        /// <returns>List of Status</returns>
        [HttpGet, Route("Status")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage StatusForCancel()
        {
            string daCancelStatusesStringRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_Cancel");
            string daCancelStatusesString = daCancelStatusesStringRaw.Trim();
            int[] cancelStasus = daCancelStatusesString.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
            List<int> res = ordersFlow.StatusForCancel(DBInfo, cancelStasus);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update's Order Status to Delivery Agent Server 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateDA_OrderStatus")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage UpdateDA_OrderStatus(List<DA_OrderStatusModel> Model)
        {
            List<ResultsAfterDA_OrderActionsModel> res = orderStatusFlow.AddNewList(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Update's Order Status to Delivery Agent Server From HitPos.Order table
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateDA_OrderStatusFromHitPos")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage UpdateDA_OrderStatusFromHitPos(List<DA_OrderStatusModel> Model)
        {
            //From HitPos field OrderDAId contains da_order.OrderNo. Replace with DA_Order.Id field
            foreach (DA_OrderStatusModel item in Model)
            {
                DA_OrderModel order = ordersFlow.GetOrderByOrderNo(DBInfo, item.OrderDAId);
                if (order != null)
                    item.OrderDAId = order.Id;
            }

            //Updates da_order statuses
            List<ResultsAfterDA_OrderActionsModel> res = orderStatusFlow.AddNewList(DBInfo, Model);

            //On results changes the da_orderid to da_orderno
            foreach (ResultsAfterDA_OrderActionsModel item in res)
            {
                DA_ExtOrderModel order = ordersFlow.GetOrderById(DBInfo, item.DA_Order_Id);
                item.DA_Order_Id = order.OrderModel.OrderNo ?? 0;
            }

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Method get to return a model with orders and statuses need to be cheched via Goodys Omnirest
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetOrderStatusesForGoodysOmniRest")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetOrderStatusesForGoodysOmniRest()
        {
            List<DA_OrderStatusModel> completed;

            //1. Get a list of orders to check status and a list of orders opened more than 24 hours
            List<DA_OrdersForGoodysOmnirestStatus> res = ordersFlow.GetOrdersForGoodysOmnirest(DBInfo, out completed);

            //2. if orders opened more than 24 hours exists then close them
            if(completed != null && completed.Count > 0)
            {
                List<ResultsAfterDA_OrderActionsModel> updResults = orderStatusFlow.AddNewList(DBInfo, completed);
                //2.1 Not all orders closed
                if (updResults.FindAll(f=> f.Succeded==false).Count > 0)
                {
                    foreach (ResultsAfterDA_OrderActionsModel item in updResults.FindAll(f => f.Succeded == false))
                        logger.Error("Order Id " + item.DA_Order_Id.ToString() + " faild. " + item.Errors + " \r\n");
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// Converts and updates a list of  Goodys Omnirest statuses 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateDA_OrderStatusesForGoodysOmnirest")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage UpdateDA_OrderStatusesForGoodysOmnirest(List<DA_OrdersForGoodysOmnirestStatus> model)
        {
            //1. Convert Goodys Omnirest statuses to da_order statuses
            List<DA_OrderStatusModel> daOrderStatus = ordersFlow.ConvertOmniRestStatusToDA_OrderStatus(model);

            //2. Update da_order statuses
            List<ResultsAfterDA_OrderActionsModel> updResults = orderStatusFlow.AddNewList(DBInfo, daOrderStatus);
            
            //3 Not all orders closed
            if (updResults.FindAll(f => f.Succeded == false).Count > 0)
            {
                foreach (ResultsAfterDA_OrderActionsModel item in updResults.FindAll(f => f.Succeded == false))
                    logger.Error("Order Id " + item.DA_Order_Id.ToString() + " faild. " + item.Errors + " \r\n");
            }

            return Request.CreateResponse(HttpStatusCode.OK, updResults.FindAll(f => f.Succeded == false).Count == 0);
        }


        [HttpPost, Route("UpdateExternalPayment")]
        [AllowAnonymous]
        [IsDA]
        [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage UpdateExternalPayment(ExternalPaymentModel model)
        {
            long orderId = ordersFlow.UpdateExternalPayment(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        /// <summary>
        /// Post Web Goodys Orders model from ATCOM/OMNIREST
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("PostWebGoodysOrder")]
        [AllowAnonymous]
        [IsDA]
        public HttpResponseMessage PostWebGoodysOrder(WebGoodysOrdersModel Model)
        {
            bool res = ordersFlow.PostWebGoodysOrder(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}