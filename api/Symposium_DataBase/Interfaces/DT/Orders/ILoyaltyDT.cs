using Symposium.Models.Models;
using Symposium.Models.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Orders
{
    public interface ILoyaltyDT
    {
        /// <summary>
        /// Returns a list of Loyalty based on LoyaltyId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="LoyalltyId"></param>
        /// <returns></returns>
        List<LoyaltyModel> GetLoyaltyByLoyalltyId(DBInfoModel Store, string LoyalltyId);

        /// <summary>
        /// Returns a list of Loyalty based on DAOrderId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="DAOrderId"></param>
        /// <returns></returns>
        List<LoyaltyModel> GetLoyaltyByDAOrderID(DBInfoModel Store, long DAOrderId);

        /// <summary>
        /// Returns a list of Loyalty based on Invoicesid
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        List<LoyaltyModel> GetLoyaltyByInvoicesId(DBInfoModel Store, long InvoicesId);

        /// <summary>
        /// Returns a list of Loyalty based on Coupon Code
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CouponCode"></param>
        /// <returns></returns>
        List<LoyaltyModel> GetLoyaltyByCouponCode(DBInfoModel Store, string CouponCode);

        /// <summary>
        /// Returns a list of Loyalty based on Gift Card Code
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="GiftcardCode"></param>
        /// <returns></returns>
        List<LoyaltyModel> GetLoyaltyByGiftcardCode(DBInfoModel Store, string GiftcardCode);

        /// <summary>
        /// Returns a list of Loyalty based on Channel
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Channel"></param>
        /// <returns></returns>
        List<LoyaltyModel> GetLoyaltyByChannel(DBInfoModel Store, string Channel);

        /// <summary>
        /// Returns a list of Loyalty based on Coupon Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CouponType"></param>
        /// <returns></returns>
        List<LoyaltyModel> GetLoyaltyByCouponType(DBInfoModel Store, string CouponType);

        /// <summary>
        /// update loyalty model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModel(DBInfoModel Store, LoyaltyModel item);

        /// <summary>
        /// Insert new loyalty on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long InsertModel(DBInfoModel Store, LoyaltyModel item);

        /// <summary>
        /// Delete a loyalty from db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        int DeleteModel(DBInfoModel Store, long Id);
    }
}
