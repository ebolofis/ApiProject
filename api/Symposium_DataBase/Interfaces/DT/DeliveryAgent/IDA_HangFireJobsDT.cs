using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_HangFireJobsDT
    {
        /// <summary>
        /// Get's Order To Send To Clients
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        OrderFromDAToClientForWebCallModel GetOrdersToSend(DBInfoModel Store, long DaOrderId);


        /// <summary>
        /// Geting Customer With list of addresses from DA Server
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CustomerId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        List<DASearchCustomerModel> GetCustomer(DBInfoModel Store, List<long> CustomerId, ExternalSystemOrderEnum ExtType);


        /// <summary>
        /// Retruns Da Store Model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        DA_StoreModel GetStore(DBInfoModel Store, long StoreId);

        /// <summary>
        /// Return's s list of da_stores
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        List<DA_StoreModel> GetStoresList(DBInfoModel Store);

        /// <summary>
        /// Update's Order From Server with IsSend Action
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        bool UpdateOrderWithSendStatus(DBInfoModel Store, long OrderId, long StoreOrderId, long StoreOrderNo, short StoreStatus, out string Error);

        /// <summary>
        /// Delete Keys from DA Scheduler Taskes
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="sKeys"></param>
        /// <returns></returns>
        bool DeleteSchedulerKeys(DBInfoModel Store, string sKeys);

        /// <summary>
        /// Delete's Records from Scheduled Taskes with FaildNo bigger than DelAfter parameter
        /// FaildNo Number of Failed
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DelAfter"></param>
        void DeleteSchedulerAfterFaild(DBInfoModel Store, int DelAfter);

        /// <summary>
        /// Return's all DA Order Ids to Send To Stores
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        List<long> GetDAOrderIdsToSend(DBInfoModel Store);

        /// <summary>
        /// Update DA_Order With Error Message
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <param name="ErrorMess"></param>
        /// <returns></returns>
        bool SetErrorToDA_Order(DBInfoModel Store, long OrderId, string ErrorMess);

        /// <summary>
        /// Increase FailNo to not succeded records
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="StoreId"></param>
        /// <param name="ShortId"></param>
        /// <returns></returns>
        bool UpdateFaildNos(DBInfoModel Store, long StoreId, int ShortId);
    }

    
}
