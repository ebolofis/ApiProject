using Symposium.Models.Enums;
using Symposium.Models.Models.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.ExternalSystems
{
    public class ICouponFlows : IiCouponFlows
    {
        IiCouponTasks ictask;
        public ICouponFlows(IiCouponTasks _ictask)
        {
            ictask = _ictask;
        }

        /// <summary>
        /// Provide dbo.AccountType configuration to match Split Payment on TenderCoupon policy
        /// </summary>
        /// <returns></returns>
        public string GetCouponsAccountType() => ictask.ConstructURI(ICouponApiSettingsEnum.AccountType);


        /// <summary>
        /// Provided a coupon code triggers task to Call iCoupon API with params and auth to return a list of valid and invalid coupons 
        /// </summary>
        /// <param name="barcodestr"></param>
        /// <returns></returns>
        public CouponsResponse GetCouponsFlow(string barcodestr, string barcodeFormat, string tillRef, string locationRef,string serviceProviderRef, string tradingOutletRef)  => ictask.GetCouponsTask(barcodestr, barcodeFormat, tillRef, locationRef, serviceProviderRef, tradingOutletRef);



        /// <summary>
        /// Based on Chosen Coupon and action  prefered triggers Task to call API either to use coupon or void it back
        /// </summary>
        /// <param name="coupon"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public ICoupon UpdateCoupon(ICoupon coupon, string action, string tillRef, string locationRef, string serviceProviderRef, string tradingOutletRef, string tradingOutletName) => ictask.UpdateCoupon(coupon, action, tillRef, locationRef, serviceProviderRef, tradingOutletRef, tradingOutletName);


        /// <summary>
        /// Provide sales data for a redeemed coupon
        /// </summary>
        /// <param name="insertData"></param>
        /// <returns></returns>
        public IcouponInsertReceiptsDataModel InsertSalesData(IcouponInsertReceiptsDataModel insertData) => ictask.InsertSalesData(insertData);
    }
}
