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
    public class OrderDetailTasks : IOrderDetailTasks
    {
        IOrderDetailsDT dt;

        public OrderDetailTasks(IOrderDetailsDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Add's new order Detail
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddOrderDetail(DBInfoModel Store, OrderDetailModel item)
        {
            return dt.AddOrderDetail(Store, item);
        }

        /// <summary>
        /// Returns an OrderDetail using Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public OrderDetailModel GetOrderDetailById(DBInfoModel Store, long Id)
        {
            return dt.GetOrderDetailById(Store, Id);
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
            return dt.SetStatusToOrderDetails(Store, OrderId, Status);
        }
    }
}
