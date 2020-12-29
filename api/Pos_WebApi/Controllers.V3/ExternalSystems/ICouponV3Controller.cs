using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.ExternalSystems;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.ExternalSystems
{
    [RoutePrefix("api/v3/ICoupon")]

    public class ICouponV3Controller : BasicV3Controller
    {
        IiCouponFlows icflow;

        public ICouponV3Controller(IiCouponFlows _icflow) {
            icflow = _icflow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BarcodeModel"></param>
        /// <returns></returns>
        [HttpPost, Route("GetCouponsByBarcode")]
        public HttpResponseMessage GetCouponsByBarcode(BarcodeModel barModel)
        {
            string barcodeFormat = MainConfigurationHelper.GetSubConfiguration("api", "barcodeFormat");
            string tillRef = MainConfigurationHelper.GetSubConfiguration("api", "tillRef");
            string locationRef = MainConfigurationHelper.GetSubConfiguration("api", "locationRef");
            string serviceProviderRef = MainConfigurationHelper.GetSubConfiguration("api", "serviceProviderRef");
            string tradingOutletRef = MainConfigurationHelper.GetSubConfiguration("api", "tradingOutletRef");

            var res = icflow.GetCouponsFlow(barModel.barcodestr, barcodeFormat, tillRef, locationRef, serviceProviderRef, tradingOutletRef);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barcodestr"></param>
        /// <returns></returns>
        [HttpPost, Route("UpdateCoupon/{actionChoice}")]
        public HttpResponseMessage UpdateCoupon(string actionChoice, ICoupon coupon)
        {
            string tillRef = MainConfigurationHelper.GetSubConfiguration("api", "tillRef");
            string locationRef = MainConfigurationHelper.GetSubConfiguration("api", "locationRef");
            string serviceProviderRef = MainConfigurationHelper.GetSubConfiguration("api", "serviceProviderRef");
            string tradingOutletRef = MainConfigurationHelper.GetSubConfiguration("api", "tradingOutletRef");
            string tradingOutletName = MainConfigurationHelper.GetSubConfiguration("api", "tradingOutletName");

            var res = icflow.UpdateCoupon(coupon , actionChoice, tillRef, locationRef, serviceProviderRef, tradingOutletRef, tradingOutletName);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        /// <summary>
        /// API call Returns Configuration of tender coupon based on webconfig
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("GetCouponsAccountType")]
        public HttpResponseMessage GetCouponsAccountType() {
            string res = icflow.GetCouponsAccountType();
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
