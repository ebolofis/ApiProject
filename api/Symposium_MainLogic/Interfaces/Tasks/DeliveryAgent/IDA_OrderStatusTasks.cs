using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent
{
    public interface IDA_OrderStatusTasks
    {
        /// <summary>
        /// Insert's a New Model To DB
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewModel(DBInfoModel dbInfo, DA_OrderStatusModel item);


        /// <summary>
        /// Update statuses for DA_Order
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        void UpdateDa_OrderStatus(DBInfoModel dbInfo, DA_OrderStatusModel item);

        /// <summary>
        /// Get's a List of orders with max status onhold (based on statusdate) and hour different bwtween now and statusdate bigger than 2
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        List<long> GetOnHoldOrdersForDelete(DBInfoModel Store, int delMinutes);
    }
}
