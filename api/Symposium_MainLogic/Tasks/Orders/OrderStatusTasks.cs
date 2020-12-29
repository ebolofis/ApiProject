using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class OrderStatusTasks : IOrderStatusTasks
    {
        IOrderStatusDT dt;

        public OrderStatusTasks(IOrderStatusDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Add new record to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddOrderStatus(DBInfoModel Store, OrderStatusModel item)
        {
            return dt.AddOrderStatus(Store, item);
        }

        /// <summary>
        /// Return's all not send order status to inform DA
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public List<OrderStatusModel> GetNotSendStatus(DBInfoModel Store, ExternalSystemOrderEnum extType)
        {
            return dt.GetNotSendStatus(Store, extType);
        }

        /// <summary>
        /// Update's a model to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateOrderStatus(DBInfoModel Store, OrderStatusModel item)
        {
            return dt.UpdateOrderStatus(Store, item);
        }

        /// <summary>
        /// Get's an OrderStatus Model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public OrderStatusModel GetOrderStatusById(DBInfoModel Store, long Id)
        {
            return dt.GetOrderStatusById(Store, Id);
        }

        /// <summary>
        /// Update a list of OrderStatus to IsSend = Parameter IsSend using List Of Order Status Ids
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Ids"></param>
        /// <param name="IsSend"></param>
        public void UpdateListOfOrderStatusToIsSendById(DBInfoModel Store, List<long> Ids, bool IsSend)
        {
            dt.UpdateListOfOrderStatusToIsSendById(Store, Ids, IsSend);
        }

        /// <summary>
        /// Return's Last Order Status for an Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public OrderStatusModel GetLastOrdrStatusForOrderId(DBInfoModel Store, long OrderId)
        {
            return dt.GetLastOrdrStatusForOrderId(Store, OrderId);
        }

        /// <summary>
        /// Create order status 'complete' for order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public OrderStatusModel CreateOrderStatusFromOrder(OrderModel order, long? staffId, OrderStatusEnum status)
        {
            OrderStatusModel orderStatus = new OrderStatusModel();
            orderStatus.Status = status;
            orderStatus.TimeChanged = DateTime.Now;
            orderStatus.OrderId = order.Id;
            orderStatus.StaffId = staffId;
            orderStatus.ExtState = order.ExtType;//(int)ExternalSystemOrderEnum.DeliveryAgent;
            orderStatus.IsSend = false;
            orderStatus.DAOrderId = Convert.ToInt64(order.ExtKey);
            return orderStatus;
        }
    }
}
