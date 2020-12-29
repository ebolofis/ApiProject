using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IOrderDetailFlows
    {

        /// <summary>
        /// Add's new order Detail
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddOrderDetail(DBInfoModel dbInfo, OrderDetailModel item);

        /// <summary>
        /// Gets order ids of order detail invoices that are dine in
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="orderDetailInvoices"></param>
        /// <returns></returns>
        List<long> GetOrderIdsFromOrderDetailInvoices(DBInfoModel dbInfo, List<OrderDetailInvoicesModel> orderDetailInvoices);

        /// <summary>
        /// Update OrderDetail set status for a specific Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        bool SetStatusToOrderDetails(DBInfoModel Store, long OrderId, OrderStatusEnum Status);
    }
}
