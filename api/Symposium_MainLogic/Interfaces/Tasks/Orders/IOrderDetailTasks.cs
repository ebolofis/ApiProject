using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IOrderDetailTasks
    {
        /// <summary>
        /// Add's new order Detail
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddOrderDetail(DBInfoModel Store, OrderDetailModel item);

        /// <summary>
        /// Returns an OrderDetail using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        OrderDetailModel GetOrderDetailById(DBInfoModel Store, long Id);

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
