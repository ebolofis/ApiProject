using Microsoft.AspNet.SignalR;
using Pos_WebApi.Helpers.V3;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Symposium.Models.Models.Orders;
using Symposium.Helpers;

namespace Pos_WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/Orders")]
    public class OrderV3Controller : BasicV3Controller
    {
        IPosInfoFlows posInfoFlow;
        IOrderFlows orderFlows;
        IHubContext hub = GlobalHost.ConnectionManager.GetHubContext<Hubs.WebPosHub>();
        IOrderStatusFlows orderStatusFlow;
        IInvoicesFlows invoiceFlows;

        public OrderV3Controller(IPosInfoFlows posInfoFlow, IOrderFlows orderFlows, IOrderStatusFlows orderStatusFlow, IInvoicesFlows invoiceFlows)
        {
            this.posInfoFlow = posInfoFlow;
            this.orderFlows = orderFlows;
            this.orderStatusFlow = orderStatusFlow;
            this.invoiceFlows = invoiceFlows;
        }

        /// <summary>
        /// Upsert a list of Accounts from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("InsertDeliveryOrders")]
        [System.Web.Http.Authorize]
        public HttpResponseMessage InsertDeliveryOrders(OrderFromDAToClientForWebCallModel model)
        {
            SignalRAfterInvoiceModel signal = null;

            List<ResultsAfterDA_OrderActionsModel> OrderResults = new List<ResultsAfterDA_OrderActionsModel>();
            string extecrName = "";

            List<FailedDA_OrdersModels> FaildOrders = new List<FailedDA_OrdersModels>();
            DeliveryAgentHelper hlp = new DeliveryAgentHelper();

            extecrName = posInfoFlow.GetExtEcrName(DBInfo, model.StoreModel.PosId ?? 0);

            if (string.IsNullOrEmpty(extecrName))
                FaildOrders.Add(new FailedDA_OrdersModels { Order = model.Order, ErrorMess = "No ExtEcr exists" });
            else
          if (!hlp.CheckConnectedExtecr(extecrName))
                FaildOrders.Add(new FailedDA_OrdersModels { Order = model.Order, ErrorMess =  string.Format(Symposium.Resources.Errors.EXTECRNOTCONNECTED, (extecrName ?? "<NULL>")) });
            //else
            //if (!orderFlows.CheckClientStoreOrderStatus(DBInfo, model.StoreModel.Id ?? 0, (OrderTypeStatus)model.Order.OrderType))
            //    FaildOrders.Add(new FailedDA_OrdersModels { Order = model.Order, ErrorMess = "Client Store not support order type" });

            if (FaildOrders.Count != 0)
                OrderResults.Add(new ResultsAfterDA_OrderActionsModel()
                {
                    DA_Order_Id = model.Order.Id,
                    Store_Order_Id = model.Order.StoreOrderId,
                    Store_Order_No = model.Order.StoreOrderNo ?? 0,
                    Store_Order_Status = model.Order.Status,
                    Store_Order_Status_DT = DateTime.Now,
                    Succeded = false,
                    Errors = FaildOrders[0].ErrorMess,
                    ExtEcrName = extecrName,
                    InvoiceId = -1,
                    PrintType = PrintTypeEnum.PrintWhole,
                    ItemAdditionalInfo = "",
                    TempPrint = false
                });
            else
            {
                bool createDP = true;
                if (model.Order.OrderType == OrderTypeStatus.DineIn && 
                                            (model.Order.AccountType == (short)AccountTypeEnum.CreditCard || model.Order.AccountType == (short)AccountTypeEnum.TicketCompliment) && 
                                            model.Order.IsPaid)
                {
                    createDP = false;
                }
                OrderResults = orderFlows.InsertDeliveryOrders(DBInfo, model, "", createDP, out signal);
             
            }
                

            foreach (ResultsAfterDA_OrderActionsModel item in OrderResults)
            {
                if (item.Succeded && item.Store_Order_Status == OrderStatusEnum.Received)
                {
                    if (item.Old_Store_Order_Status != (int)item.Store_Order_Status)
                        hub.Clients.Group(DBInfo.Id.ToString()).daUpdateOrdertoPos(item.Store_Order_Id, item.Old_Store_Order_Status, (int)item.Store_Order_Status);

                    if (signal != null)
                    {
                        if (signal.TableId != null)
                        {
                            hub.Clients.Group(DBInfo.Id.ToString()).refreshTable(DBInfo.Id, signal.TableId);
                        }
                    }
                }
                else if (item.Succeded && item.Store_Order_Status == OrderStatusEnum.Preparing)
                {
                    hub.Clients.Group(DBInfo.Id.ToString()).newReceipt(DBInfo.Id.ToString() + "|" + item.ExtEcrName, item.InvoiceId, true, true,
                    item.PrintType, item.ItemAdditionalInfo, item.TempPrint);

                    if (item.Old_Store_Order_Status != (int)item.Store_Order_Status)
                        hub.Clients.Group(DBInfo.Id.ToString()).daUpdateOrdertoPos(item.Store_Order_Id, item.Old_Store_Order_Status, (int)item.Store_Order_Status);

                    if (signal != null)
                    {
                        if (signal.TableId != null)
                        {
                            hub.Clients.Group(DBInfo.Id.ToString()).refreshTable(DBInfo.Id, signal.TableId);
                        }
                    }
                }
                else if (item.Succeded && item.Store_Order_Status == OrderStatusEnum.Canceled)
                {
                    if (signal != null)
                    {
                        hub.Clients.Group(DBInfo.Id.ToString()).newReceipt(DBInfo.Id + "|" + signal.ExtecrName, signal.InvoiceId, signal.useFiscalSignature,
                            signal.SendsVoidToKitchen, signal.PrintType, signal.ItemAdditionalInfo, signal.TempPrint);
                        string customerClassType = MainConfigurationHelper.GetSubConfiguration("api", "CustomerClass");
                        if (customerClassType == "Pos_WebApi.Customer_Modules.Goodies")
                        {
                            hub.Clients.Group(DBInfo.Id.ToString()).NewInvoice(model.Order.PosId, signal.InvoiceId);
                            hub.Clients.All.OpenOrders("Get Orders after insertion of a new order");
                        }
                        if (signal.TableId != null)
                        {
                            hub.Clients.Group(DBInfo.Id.ToString()).refreshTable(DBInfo.Id, signal.TableId);
                        }
                        hub.Clients.Group(DBInfo.Id.ToString()).kdsMessage(DBInfo.Id, signal.kdsMessage);
                        hub.Clients.Group(DBInfo.Id.ToString()).deliveryMessage(DBInfo.Id, signal.deliveryMessage, signal.SalesTypes);
                        if (item.Old_Store_Order_Status != (int)item.Store_Order_Status)
                            hub.Clients.Group(DBInfo.Id.ToString()).daUpdateOrdertoPos(item.Store_Order_Id, item.Old_Store_Order_Status, (int)item.Store_Order_Status);
                        hub.Clients.All.OpenOrders("Get Orders after insertion of a new order");
                    }
                }
                else if(item.Succeded && item.Store_Order_Status == OrderStatusEnum.Complete)
                {
                    hub.Clients.Group(DBInfo.Id.ToString()).newReceipt(DBInfo.Id.ToString() + "|" + item.ExtEcrName, item.InvoiceId, true, true,
                    item.PrintType, item.ItemAdditionalInfo, item.TempPrint);

                    if (item.Old_Store_Order_Status != (int)item.Store_Order_Status)
                        hub.Clients.Group(DBInfo.Id.ToString()).daUpdateOrdertoPos(item.Store_Order_Id, item.Old_Store_Order_Status, (int)item.Store_Order_Status);

                    if (signal != null)
                    {
                        if (signal.TableId != null)
                        {
                            hub.Clients.Group(DBInfo.Id.ToString()).refreshTable(DBInfo.Id, signal.TableId);
                        }
                    }
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, OrderResults);


        }

        /// <summary>
        /// Get's all Isdelayed order with status Preparing and check if are available to send to kitchen.
        /// Available are all order's with EstTakeoutDate bigger than datetime.now minus EstimTime
        /// </summary>
        /// <param name="EstimTime"></param>
        /// <returns></returns>
        [HttpPost, Route("SendDelayedOrdersToKitchen")]
        public HttpResponseMessage SendDelayedOrdersToKitchen(ResultsAfterDA_OrderActionsModel EstimTime /*int EstimTime*/)
        {
            string Error;
            List<OrderModel> DelayedOrders = orderFlows.GetDelayedOrders(DBInfo);
            foreach (OrderModel item in DelayedOrders)
            {
                //if ((item.EstTakeoutDate ?? DateTime.Now) >= DateTime.Now.AddMinutes((-1) * EstimTime.DA_Order_Id))
                if ((item.EstTakeoutDate ?? DateTime.Now).AddMinutes((-1) * EstimTime.DA_Order_Id) <= DateTime.Now)
                {
                    PosInfoModel posInfo = posInfoFlow.GetSinglePosInfo(DBInfo, item.PosId);
                    string extecrName = posInfo.FiscalName;
                    long InvoiceId = orderFlows.GetInvoiceIdByOrderId(DBInfo, item.Id ?? 0);

                    int oldOrderStatus = (int)orderStatusFlow.GetLastOrderStatusForOrderId(DBInfo, item.Id ?? 0).Status;

                    if ((FiscalTypeEnum)posInfo.FiscalType == FiscalTypeEnum.Generic)
                    {
                       if (!invoiceFlows.UpdateInvoicePrinted(DBInfo, InvoiceId, true, out Error))
                        {
                            logger.Error("SendDelayedOrdersToKitchen : " + Error);
                            continue;
                        }
                    }
                    
                    /*Order Status */
                    OrderStatusModel OrderStatus = new OrderStatusModel()
                    {
                        Status = OrderStatusEnum.Preparing,
                        TimeChanged = DateTime.Now,
                        StaffId = item.StaffId,
                        IsSend = false,
                        OrderId = item.Id,
                        DAOrderId = long.Parse(string.IsNullOrEmpty(item.ExtKey) ? "0" : item.ExtKey),
                        ExtState = item.ExtType
                    };
                    OrderStatus.Id = orderStatusFlow.AddOrderStatus(DBInfo, OrderStatus);
                    orderFlows.SetStatusToOrderDetails(DBInfo, item.Id ?? 0, OrderStatusEnum.Preparing);


                    hub.Clients.Group(DBInfo.Id.ToString()).newReceipt(DBInfo.Id.ToString() + "|" + extecrName, InvoiceId, true, true,
                        PrintTypeEnum.PrintWhole, null, false);

                    if (oldOrderStatus != (int)OrderStatusEnum.Preparing)
                        hub.Clients.Group(DBInfo.Id.ToString()).daUpdateOrdertoPos(item.Id ?? 0, oldOrderStatus, (int)OrderStatusEnum.Preparing);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// call flow to handle card payment for specific order
        /// </summary>
        /// <param name="model">PluginCardPaymentModel</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpPost, Route("OrderCardPayment")]
        public HttpResponseMessage OrderCardPayment(PluginCardPaymentModel model )
        {
            bool res =  orderFlows.OrderCardPayment(model, DBInfo);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}