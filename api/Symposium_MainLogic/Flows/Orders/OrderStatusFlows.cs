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
    public class OrderStatusFlows : IOrderStatusFlows
    {
        IOrderStatusTasks task;

        public OrderStatusFlows(IOrderStatusTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// Add new record to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddOrderStatus(DBInfoModel dbInfo, OrderStatusModel item)
        {
            return task.AddOrderStatus(dbInfo, item);
        }

        /// <summary>
        /// Return's all not send order status to inform DA
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="extType"></param>
        /// <returns></returns>
        public List<OrderStatusModel> GetNotSendStatus(DBInfoModel dbInfo, ExternalSystemOrderEnum extType)
        {
            return task.GetNotSendStatus(dbInfo, extType);
        }

        /// <summary>
        /// Update's a model to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateOrderStatus(DBInfoModel dbInfo, OrderStatusModel item)
        {
            return task.UpdateOrderStatus(dbInfo, item);
        }

        /// <summary>
        /// Get's an OrderStatus Model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public OrderStatusModel GetOrderStatusById(DBInfoModel dbInfo, long Id)
        {
            return task.GetOrderStatusById(dbInfo, Id);
        }

        /// <summary>
        /// Return's Last Order Status for an Order Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public OrderStatusModel GetLastOrderStatusForOrderId(DBInfoModel dbInfo, long OrderId)
        {
            return task.GetLastOrdrStatusForOrderId(dbInfo, OrderId);
        }
    }
}
