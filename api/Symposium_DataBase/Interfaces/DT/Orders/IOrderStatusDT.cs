using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IOrderStatusDT
    {

        /// <summary>
        /// Add new record to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        long AddOrderStatus(IDbConnection db, OrderStatusModel item, IDbTransaction dbTransact, out string Error);

        /// <summary>
        /// Add new record to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddOrderStatus(DBInfoModel Store, OrderStatusModel item);

        /// <summary>
        /// Return's all not send order status to inform DA
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        List<OrderStatusModel> GetNotSendStatus(DBInfoModel Store, ExternalSystemOrderEnum extType);


        /// <summary>
        /// Update's a model to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateOrderStatus(DBInfoModel Store, OrderStatusModel item);

        /// <summary>
        /// Get's an OrderStatus Model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        OrderStatusModel GetOrderStatusById(DBInfoModel Store, long Id);

        /// <summary>
        /// Update a list of OrderStatus to IsSend = Parameter IsSend using List Of Order Status Ids
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Ids"></param>
        /// <param name="IsSend"></param>
        void UpdateListOfOrderStatusToIsSendById(DBInfoModel Store, List<long> Ids, bool IsSend);

        /// <summary>
        /// Return's Last Order Status for an Order Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        OrderStatusModel GetLastOrdrStatusForOrderId(DBInfoModel Store, long OrderId);
    }
}
