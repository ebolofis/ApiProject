using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class OrderDetailFlows : IOrderDetailFlows 
    {
        IOrderDetailTasks task;
        
        public OrderDetailFlows(IOrderDetailTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// Add's new order Detail
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddOrderDetail(DBInfoModel dbInfo, OrderDetailModel item)
        {
            return task.AddOrderDetail(dbInfo, item);
        }

        /// <summary>
        /// Gets order ids of order detail invoices that are dine in
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="orderDetailInvoices"></param>
        /// <returns></returns>
        public List<long> GetOrderIdsFromOrderDetailInvoices(DBInfoModel dbInfo, List<OrderDetailInvoicesModel> orderDetailInvoices)
        {
            List<long> orderIds = new List<long>();
            if (orderDetailInvoices != null)
            {
                foreach(OrderDetailInvoicesModel orderDetailInvoice in orderDetailInvoices)
                {
                    OrderDetailModel orderDetail = task.GetOrderDetailById(dbInfo, orderDetailInvoice.OrderDetailId ?? 0);
                    if (orderDetail != null && orderDetail.OrderId != null && orderDetail.SalesTypeId != null && orderDetail.SalesTypeId == (long)OrderTypeStatus.DineIn)
                    {
                        bool orderFound = false;
                        foreach (long orderId in orderIds)
                        {
                            if (orderId == orderDetail.OrderId)
                            {
                                orderFound = true;
                                break;
                            }
                        }
                        if (!orderFound)
                        {
                            orderIds.Add(orderDetail.OrderId ?? 0);
                        }
                    }
                }
            }
            return orderIds;
        }

        /// <summary>
        /// Update OrderDetail set status for a specific Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public bool SetStatusToOrderDetails(DBInfoModel Store, long OrderId, OrderStatusEnum Status)
        {
            return task.SetStatusToOrderDetails(Store, OrderId, Status);
        }
    }
}
