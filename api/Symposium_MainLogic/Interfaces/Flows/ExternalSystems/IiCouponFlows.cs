using Symposium.Models.Models.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems
{
    public interface IiCouponFlows
    {
        /// <summary>
        /// Provide dbo.AccountType configuration to match Split Payment on TenderCoupon policy
        /// </summary>
        /// <returns></returns>
        string GetCouponsAccountType();

        /// <summary>
        /// Provided a coupon code triggers task to Call iCoupon API with params and auth to return a list of valid and invalid coupons 
        /// </summary>
        /// <param name="barcodestr"></param>
        /// <returns></returns>
        CouponsResponse GetCouponsFlow(string barcodestr, string barcodeFormat, string tillRef, string locationRef, string serviceProviderRef, string tradingOutletRef);

        /// <summary>
        /// Based on Chosen Coupon and action prefered triggers Task to call API either to use coupon or void it back
        /// </summary>
        /// <param name="coupon"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        ICoupon UpdateCoupon(ICoupon coupon, string action, string tillRef, string locationRef, string serviceProviderRef, string tradingOutletRef, string tradingOutletName);


        /// <summary>
        /// Provide sales data for a redeemed coupon
        /// </summary>
        /// <param name="insertData"></param>
        IcouponInsertReceiptsDataModel InsertSalesData(IcouponInsertReceiptsDataModel insertData);
    }
}
