using Symposium.Models.Models;
using Symposium.Models.Models.Orders;
using Symposium.WebApi.DataAccess.Interfaces.DT.Orders;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.Orders
{
    public class LoyaltyTasks : ILoyaltyTasks
    {
        ILoyaltyDT dt;

        public LoyaltyTasks(ILoyaltyDT _dt)
        {
            dt = _dt;
        }

        /// <summary>
        /// Delete a loyalty from db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public int DeleteModel(DBInfoModel Store, long Id)
        {
            return dt.DeleteModel(Store, Id);
        }

        /// <summary>
        /// Returns a list of Loyalty based on Channel
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Channel"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByChannel(DBInfoModel Store, string Channel)
        {
            return dt.GetLoyaltyByChannel(Store, Channel);
        }

        /// <summary>
        /// Returns a list of Loyalty based on Coupon Code
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CouponCode"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByCouponCode(DBInfoModel Store, string CouponCode)
        {
            return dt.GetLoyaltyByCouponCode(Store, CouponCode);
        }

        /// <summary>
        /// Returns a list of Loyalty based on Coupon Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CouponType"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByCouponType(DBInfoModel Store, string CouponType)
        {
            return dt.GetLoyaltyByCouponType(Store, CouponType);
        }

        /// <summary>
        /// Returns a list of Loyalty based on DAOrderId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByDAOrderID(DBInfoModel Store, long DAOrderId)
        {
            return dt.GetLoyaltyByDAOrderID(Store, DAOrderId);
        }

        /// <summary>
        /// Returns a list of Loyalty based on Gift Card Code
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="GiftcardCode"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByGiftcardCode(DBInfoModel Store, string GiftcardCode)
        {
            return dt.GetLoyaltyByGiftcardCode(Store, GiftcardCode);
        }

        /// <summary>
        /// Returns a list of Loyalty based on Invoicesid
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByInvoicesId(DBInfoModel Store, long InvoicesId)
        {
            return dt.GetLoyaltyByInvoicesId(Store, InvoicesId);
        }

        /// <summary>
        /// Returns a list of Loyalty based on LoyaltyId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="LoyalltyId"></param>
        /// <returns></returns>
        public List<LoyaltyModel> GetLoyaltyByLoyalltyId(DBInfoModel Store, string LoyalltyId)
        {
            return dt.GetLoyaltyByLoyalltyId(Store, LoyalltyId);
        }

        /// <summary>
        /// Insert new loyalty on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, LoyaltyModel item)
        {
            return dt.InsertModel(Store, item);
        }

        /// <summary>
        /// update loyalty model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel Store, LoyaltyModel item)
        {
            return dt.UpdateModel(Store, item);
        }
    }
}
