using log4net;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_OrderStatusFlows : IDA_OrderStatusFlows
    {
        IDA_OrderStatusTasks task;
        IOrderDetailFlows orderDetailFlows;
        IOrderTask orderTasks;
        IOrderStatusTasks orderStatusTasks;
        IInvoiceTasks invoiceTasks;
        IOrderDetailInvoicesTasks orderDetailInvoicesTasks;
        IInvoiceTypeTasks invoiceTypeTasks;
        protected ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DA_OrderStatusFlows(IDA_OrderStatusTasks task, IOrderDetailFlows orderDetailFlows, IOrderTask orderTasks, IOrderStatusTasks orderStatusTasks, IInvoiceTasks invoiceTasks, IOrderDetailInvoicesTasks orderDetailInvoicesTasks, IInvoiceTypeTasks invoiceTypeTasks)
        {
            this.task = task;
            this.orderDetailFlows = orderDetailFlows;
            this.orderTasks = orderTasks;
            this.orderStatusTasks = orderStatusTasks;
            this.invoiceTasks = invoiceTasks;
            this.orderDetailInvoicesTasks = orderDetailInvoicesTasks;
            this.invoiceTypeTasks = invoiceTypeTasks;
        }

        /// <summary>
        /// Insert's a New Model To DB
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewModel(DBInfoModel dbInfo, DA_OrderStatusModel item)
        {
            return task.AddNewModel(dbInfo, item);
        }


        /// <summary>
        /// Insert's a list of DA_OrderStatus and return's Succeded and not
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ResultsAfterDA_OrderActionsModel> AddNewList(DBInfoModel dbInfo, List<DA_OrderStatusModel> model)
        {
            List<ResultsAfterDA_OrderActionsModel> res = new List<ResultsAfterDA_OrderActionsModel>();
            string Error = "";
            long newId;
            foreach (DA_OrderStatusModel item in model)
            {
                item.StatusDate = !item.KeepStoreTimeStatus ? DateTime.Now : item.StatusDate;
                ResultsAfterDA_OrderActionsModel tmp = new ResultsAfterDA_OrderActionsModel();
                newId = task.AddNewModel(dbInfo, item);

                tmp.DA_Order_Id = item.OrderDAId;
                tmp.Store_Order_Status = (OrderStatusEnum)item.Status;
                tmp.Store_Order_Status_DT = item.StatusDate;
                if (newId < 1)
                {
                    tmp.Errors = Error;
                    tmp.Succeded = false;
                    tmp.Store_Order_No = -1;
                }
                else
                {
                    tmp.Store_Order_No = newId;
                    tmp.Errors = "";
                    tmp.Succeded = true;

                    /*Update statuses for DA_Order*/
                    task.UpdateDa_OrderStatus(dbInfo, item);
                }
                res.Add(tmp);
                System.Threading.Thread.Sleep(50);
            }

            return res;
        }

        /// <summary>
        /// Inserts order status for dine in orders from invoice id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="invoiceId"></param>
        public void InsertOrderStatusFromInvoiceId(DBInfoModel dbInfo, long invoiceId)
        {
            try
            {
                InvoiceModel invoice = invoiceTasks.GetInvoiceById(dbInfo, invoiceId);
                if (invoice != null)
                {
                    InvoiceTypeModel invoiceType = invoiceTypeTasks.GetSingleInvoiceType(dbInfo, invoice.InvoiceTypeId ?? 0);
                    if (invoiceType != null && invoiceType.Type != null)
                    {
                        List<OrderDetailInvoicesModel> orderDetailInvoices = orderDetailInvoicesTasks.GetOrderDetailInvoicesOfSelectedInvoice(dbInfo, invoiceId);
                        List<long> orderIds = orderDetailFlows.GetOrderIdsFromOrderDetailInvoices(dbInfo, orderDetailInvoices);
                        foreach (long orderId in orderIds)
                        {
                            OrderModel order = orderTasks.GetOrderById(dbInfo, orderId);
                            if (order != null)
                            {
                                OrderStatusEnum status;
                                if (invoiceType.Type == (short)InvoiceTypesEnum.VoidOrder || invoiceType.Type == (short)InvoiceTypesEnum.Void)
                                {
                                    status = OrderStatusEnum.Canceled;
                                }
                                else
                                {
                                    status = OrderStatusEnum.Complete;
                                }
                                OrderStatusModel orderStatus = orderStatusTasks.CreateOrderStatusFromOrder(order, invoice.StaffId, status);
                                long orderStatusId = orderStatusTasks.AddOrderStatus(dbInfo, orderStatus);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error creating order status from order from invoice id: " + ex.ToString());
            }
        }
    }
}
