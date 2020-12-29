using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_OrderStatusDT
    {

        /// <summary>
        /// Insert's a New Model To DB
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewModel(DBInfoModel Store, DA_OrderStatusModel item);


        /// <summary>
        /// Update statuses for DA_Order
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        void UpdateDa_OrderStatus(DBInfoModel Store, DA_OrderStatusModel item);

        /// <summary>
        /// Get's a list of DAOrderStatuses
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <returns></returns>
        List<DA_OrderStatusModel> GetDA_OrderStatusesByOrderId(DBInfoModel Store, long DAOrderId);

        /// <summary>
        /// Delete's a list of DA_OrdrStatuses
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <param name="ErrorMess"></param>
        /// <returns></returns>
        bool DeleteDA_OrderStatusList(DBInfoModel Store, List<DA_OrderStatusModel> model, out string ErrorMess);

        /// <summary>
        /// Get's a List of orders with max status onhold (based on statusdate) and hour different bwtween now and statusdate bigger than 2
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        List<long> GetOnHoldOrdersForDelete(DBInfoModel Store, int delMinutes);
    }
}
