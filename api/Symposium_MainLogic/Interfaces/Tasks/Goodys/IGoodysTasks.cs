using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.Goodys
{
    public interface IGoodysTasks
    {
        OrderModel GetGoodysExternalOrderID(long InvoiceId, DBInfoModel dbInfo);
        void CancelGoodysOrderBasedOnExternalOrderId(string externalOrderId);
        LoyaltyCoupons GetCouponInfoFromOrder(DA_OrderModel order);
        bool RedeemLoyalty(DBInfoModel dbInfo, DA_OrderModel order);
        bool CancelLoyalty(DBInfoModel dbInfo, string externalOrderId);
        string GetCouponDetails(DBInfoModel dbInfo, GoodysCouponDetailInfo orderInfo);
         long GetInvoiceid(DBInfoModel db, long orderno);

        /// <summary>
        /// Return's a Login responce model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="AccountId"></param>
        /// <param name="allAddresses"></param>
        /// If Param allAddress = true then shipping and billing addresses returned else only shipping addresses.
        /// Deleted addresses never returned
        /// <returns></returns>
        GoodysLoginResponceModel GetLoginResponceModel(DBInfoModel dbInfo, string AccountId, bool allAddresses = true);
         long GetOpenOrders(DBInfoModel DBInfo);


    }

}
