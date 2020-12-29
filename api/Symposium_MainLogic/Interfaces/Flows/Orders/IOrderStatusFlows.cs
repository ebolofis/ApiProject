using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IOrderStatusFlows
    {
        /// <summary>
        /// Add new record to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddOrderStatus(DBInfoModel dbInfo, OrderStatusModel item);

        /// <summary>
        /// Return's all not send order status to inform DA
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="extType"></param>
        /// <returns></returns>
        List<OrderStatusModel> GetNotSendStatus(DBInfoModel dbInfo, ExternalSystemOrderEnum extType);

        /// <summary>
        /// Update's a model to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateOrderStatus(DBInfoModel dbInfo, OrderStatusModel item);

        /// <summary>
        /// Get's an OrderStatus Model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        OrderStatusModel GetOrderStatusById(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Return's Last Order Status for an Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        OrderStatusModel GetLastOrderStatusForOrderId(DBInfoModel dbInfo, long OrderId);
    }
}
