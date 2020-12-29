using Symposium.WebApi.MainLogic.Interfaces.Flows.Goodys;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Goodys;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Newtonsoft.Json;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.DataAccess.Interfaces.DT;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class GoodysFlow : IGoodysFlow
    {
        IGoodysTasks GoodysTasks;
        IDA_OrdersTasks orderTasks;
        

        public GoodysFlow(IGoodysTasks _GoodysTasks, IDA_OrdersTasks _orderTasks)
        {
            this.GoodysTasks = _GoodysTasks;
            this.orderTasks = _orderTasks;
        }

        public void UpdateGoodysApi(long InvoiceId, DBInfoModel dbInfo)
        {
            OrderModel goodysOrder = GoodysTasks.GetGoodysExternalOrderID(InvoiceId, dbInfo);
            CancelGoodysOrder(goodysOrder);
            return;
        }
        public long GetInvoiceid(DBInfoModel db, long orderno)
        {
            return GoodysTasks.GetInvoiceid(db, orderno);
        }
        public long GetOpenOrders(DBInfoModel DBInfo)
        {
            return GoodysTasks.GetOpenOrders(DBInfo);
        }
        public void CancelGoodysOrder(OrderModel model)
        {
            GoodysTasks.CancelGoodysOrderBasedOnExternalOrderId(model.ExtObj);
        }

        public void RedeemGoodysLoyalty(DBInfoModel dbInfo, DA_OrderModel order)
        {
            LoyaltyCoupons couponInfo = GoodysTasks.GetCouponInfoFromOrder(order);
            if (couponInfo != null && (couponInfo.LoyaltyId != null || couponInfo.CouponCode != null))
            {
                bool loyaltyRedeemed = GoodysTasks.RedeemLoyalty(dbInfo, order);
            }
            return;
        }

        public void CancelGoodysLoyalty(DBInfoModel dbInfo, long orderId)
        {
            DA_OrderModel order = orderTasks.GetSingleOrderById(dbInfo, orderId);
            LoyaltyCoupons couponInfo = GoodysTasks.GetCouponInfoFromOrder(order);
            if (couponInfo != null && couponInfo.ExternalOrderId != null)
            {
                bool loyaltyCanceled = GoodysTasks.CancelLoyalty(dbInfo, couponInfo.ExternalOrderId);
            }
            return;
        }

        public string GetCouponDetails(DBInfoModel dbInfo, GoodysCouponDetailInfo orderInfo)
        {
            string couponDetails = GoodysTasks.GetCouponDetails(dbInfo, orderInfo);
            return couponDetails;
        }
    }
}
