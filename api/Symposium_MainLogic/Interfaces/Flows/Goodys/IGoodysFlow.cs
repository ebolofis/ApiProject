using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.Goodys
{
    public interface IGoodysFlow
    {
        void UpdateGoodysApi(long InvoiceId, DBInfoModel dbInfo);
        void CancelGoodysOrder(OrderModel model);
        void RedeemGoodysLoyalty(DBInfoModel dbInfo, DA_OrderModel order);
        void CancelGoodysLoyalty(DBInfoModel dbInfo, long orderId);
        string GetCouponDetails(DBInfoModel dbInfo, GoodysCouponDetailInfo orderInfo);

        long GetInvoiceid(DBInfoModel db, long orderno);

        long GetOpenOrders(DBInfoModel DBInfo);

    }

}
