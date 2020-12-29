using log4net;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.ExternalSystems
{
    public interface IiCouponTasks
    {
        /// <summary>
        /// Based on option provided as enum choice returns url of icoupon to post on API with HTTP responce
        /// </summary>
        /// <param name="option"> URL requested string </param>
        /// <returns> string of URI to API Calls </returns>
        string ConstructURI(ICouponApiSettingsEnum option);

        /// <summary>
        /// Token Request Header Information Async() 
        /// </summary>
        /// <returns> Header auth token for bearer auth to Icoupon API calls</returns>
        string RequestAuthHeader();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barcodestr"></param>
        /// <returns></returns>
        CouponsResponse GetCouponsTask(string barcodestr, string barcodeFormat, string tillRef, string locationRef, string serviceProviderRef, string tradingOutletRef);

        /// <summary>
        /// Based on action performed Updates Coupon Provided to redeem or void as redeem cancelation
        /// </summary>
        /// <param name="coupon"></param>
        /// <param name="ucaction"></param>
        ICoupon UpdateCoupon(ICoupon coupon, string ucaction, string tillRef, string locationRef, string serviceProviderRef, string tradingOutletRef, string tradingOutletName);

        /// <summary>
        /// Provide sales data for a redeemed coupon
        /// </summary>
        /// <param name="insertData"></param>
        IcouponInsertReceiptsDataModel InsertSalesData(IcouponInsertReceiptsDataModel insertData);
    }
}
