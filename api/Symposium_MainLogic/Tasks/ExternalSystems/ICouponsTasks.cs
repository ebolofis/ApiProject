using log4net;
using Symposium.Helpers;
using Symposium.Helpers.Classes;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models.ExternalSystems;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Web.Configuration;

namespace Symposium.WebApi.MainLogic.Tasks.ExternalSystems
{

    public class ICouponsTasks : IiCouponTasks
    {
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IWebApiClientHelper wapich;

        public ICouponsTasks(IWebApiClientHelper _wapich)
        {
            this.wapich = _wapich;
        }


        /// <summary>
        /// Based on option provided as enum choice returns url of icoupon to post on API with HTTP responce
        /// </summary>
        /// <param name="option"> URL requested string </param>
        /// <returns> string of URI to API Calls </returns>
        public string ConstructURI(ICouponApiSettingsEnum option)
        {
            try
            {
                string retRaw = "";
                string ret = "";
                string apiUrlRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "icouponApiRef");
                string apiUrl = apiUrlRaw.Trim();
                switch (option)
                {
                    case ICouponApiSettingsEnum.Token:
                        retRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "icouponTokenUrl");
                        ret = retRaw.Trim();
                        break;
                    case ICouponApiSettingsEnum.GetCoupons:
                        retRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "icouponGetCouponsByBarcodeUrl");
                        ret = retRaw.Trim();
                        break;
                    case ICouponApiSettingsEnum.UpdateCoupon:
                        retRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "icouponUpdateCouponUrl");
                        ret = retRaw.Trim();
                        break;
                    case ICouponApiSettingsEnum.PostSales:
                        retRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "icouponPostSalesUrl");
                        ret = retRaw.Trim();
                        break;
                    case ICouponApiSettingsEnum.AccountType:
                        long retRawLong = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "icouponAccountTypeId");
                        ret = retRawLong.ToString();
                        break;
                    default:
                        throw new Exception("ConstructURI: Not a valid url construction choice-option");
                }
                return apiUrl + ret;
            }
            catch (Exception ex) { throw ex; }
        }


        /// <summary>
        /// Token Request Header Information Async() 
        /// Create Header ForApi Calls Request and Create Header on Success 
        /// </summary>
        /// <returns> Header auth token for bearer auth to Icoupon API calls</returns>
        public string RequestAuthHeader()
        {
            string grant_typeRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "icoupon_grant_type");
            string grant_type = grant_typeRaw.Trim();
            string client_idRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "icoupon_client_id");
            string client_id = client_idRaw.Trim();
            string client_secretRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "icoupon_client_secret");
            string client_secret = client_secretRaw.Trim();

            if (String.IsNullOrEmpty(grant_type) || String.IsNullOrEmpty(grant_type) || String.IsNullOrEmpty(grant_type))
            {
                throw new Exception("Authentication Configuration has empty values. Manage Configuration File on icoupon_grant_type, icoupon_client_id, icoupon_client_secret");
            }
            Dictionary<string, string> authForm = new Dictionary<string, string> {
                { "grant_type", grant_type },
                { "client_id", client_id },
                { "client_secret", client_secret }
            };

            int returnCode = 0; string ErrorMess = ""; string requrl = ConstructURI(ICouponApiSettingsEnum.Token);
            Dictionary<string, string> header = new Dictionary<string, string> { { "Content-Type", "application/x-www-form-urlencoded" } };

            try
            {
                dynamic tokenResponse = wapich.PostFormUrlEncoded<dynamic>(requrl, authForm, header, out returnCode, out ErrorMess);
                if (returnCode > 299)
                {
                    logger.Info("<<<<<<<<<<<                      Could not get Request Header from icoupon Api : StatusError" + returnCode + "and Error Message: " + ErrorMess + "                       >>>>>>>>>>>>>>>>>>>>");
                    throw new Exception("Invalid Icoupon Request Header Result Code");
                }
                else
                {
                    CustomGenericJsonSerializers<ICouponToken> cjson = new CustomGenericJsonSerializers<ICouponToken>();
                    ICouponToken tokenmodel = cjson.GenericDeseriallize(tokenResponse);
                    return tokenmodel.access_token;
                }
            }
            catch (Exception ex) { logger.Error("Exception while Getting Header Auth from iCoupon:" + ex.ToString()); throw ex; }
        }

        /// <summary>
        /// **************POST : GetCoupons  Async()
        /// Provide a coupon barcode
        /// Creates form model to post on iCoupon Api to get coupons 
        /// Deseriallizes obj and returns an obj with property coupons with a list 
        /// of available coupons to redeem or void
        /// </summary>
        /// <param name="barcodestr"></param>
        /// <returns></returns>

        public CouponsResponse GetCouponsTask(string barcodestr, string barcodeFormat, string tillRef, string locationRef, string serviceProviderRef, string tradingOutletRef)
        {
            string authhead = RequestAuthHeader();
            Dictionary<string, string> header = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", authhead.ToString() } };

            ICouponGetCouponFormModel Body = new ICouponGetCouponFormModel()
            { 
                barcode = barcodestr, barcodeFormat = barcodeFormat,  tillRef = tillRef, locationRef = locationRef, serviceProviderRef = serviceProviderRef, tradingOutletRef = tradingOutletRef, //configuration vars
            };
            //Dictionary<string, string> form = m.ToDictionary();

            int returnCode = 0; string ErrorMess = ""; string requrl = ConstructURI(ICouponApiSettingsEnum.GetCoupons);
            try
            {
                dynamic tokenResponse = wapich.PostTextGetCoupons<dynamic>(requrl, Body, header, out returnCode, out ErrorMess);
                if (returnCode > 299)
                {
                    logger.Info("<<<<<<<<<<<                      Could not get CouponsByBarcode iCoupon  Api : StatusError" + returnCode + "and Error Message: " + ErrorMess + "                       >>>>>>>>>>>>>>>>>>>>");
                    throw new Exception("Invalid Icoupon Request CouponByBarcode ");
                }
                CustomGenericJsonSerializers<CouponsResponse> cjson = new CustomGenericJsonSerializers<CouponsResponse>();
                CouponsResponse ret = cjson.GenericDeseriallize(tokenResponse);
                return ret;
            }
            catch (Exception ex) { logger.Error("Exception while Getting Coupons by Barcode Task: " + barcodestr + " ex:" + ex.ToString()); throw ex; }
        }


        /// <summary>
        /// Based on action performed Updates Coupon Provided to redeem or void as redeem cancelation
        /// </summary>
        /// <param name="coupon"></param>
        /// <param name="ucaction"></param>
        public ICoupon UpdateCoupon(ICoupon coupon , string ucaction, string tillRef, string locationRef, string serviceProviderRef, string tradingOutletRef, string tradingOutletName) {
            string authhead = RequestAuthHeader();
            Dictionary<string, string> header = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", authhead.ToString() } };

            ICouponUpdateFormModel body = new ICouponUpdateFormModel()
            {
                action = ucaction,
                tillRef = tillRef,
                locationRef = locationRef,
                serviceProviderRef = serviceProviderRef,
                tradingOutletRef = tradingOutletRef,
                tradingOutletName = tradingOutletName //configuration vars
            };
            //Dictionary<string, string> form = m.ToDictionary();

            int returnCode = 0; string ErrorMess = "";
            string requrl = ConstructURI(ICouponApiSettingsEnum.UpdateCoupon);
            requrl += coupon.couponRef;
            try
            {
                dynamic tokenResponse = wapich.PutTextUpdateCoupon<dynamic>(requrl, body, header, out returnCode, out ErrorMess);
                if (returnCode > 299)
                {
                    logger.Info("<<<<<<<<<<<                      Could not UpdateCoupon: " + coupon.couponRef + " action:" + ucaction + " iCoupon  Api : StatusError" + returnCode + "and Error Message: " + ErrorMess + "                       >>>>>>>>>>>>>>>>>>>>");
                    throw new Exception("Invalid iCoupon Request UpdateCoupon: " + coupon.couponRef + " action:" + ucaction);
                }
                CustomGenericJsonSerializers<Redemption> cjson = new CustomGenericJsonSerializers<Redemption>();
                //CouponsResponse ret = cjson.GenericDeseriallize(tokenResponse);
                return coupon;
            }
            catch (Exception ex) { logger.Error("Exception while Updating Coupons: " + coupon.couponRef + " action:" + ucaction + " ex:" + ex.ToString()); throw ex; }
        }

        /// <summary>
        /// Provide sales data for a redeemed coupon
        /// </summary>
        /// <param name="insertData"></param>
        public IcouponInsertReceiptsDataModel InsertSalesData(IcouponInsertReceiptsDataModel insertData)
        {
            string authhead = RequestAuthHeader();
            Dictionary<string, string> header = new Dictionary<string, string> { { "Content-Type", "application/json" }, { "Authorization", authhead.ToString() } };

            string s = string.Format("{0:0.00}", insertData.redeemedValue);
            s = s.Replace(",", ".");
            ICouponInserReceiptFormModel body = new ICouponInserReceiptFormModel()
            {
                serviceProviderRef = insertData.serviceProviderRef,
                tradingOutletRef = insertData.tradingOutletRef,
                tradingOutletName = insertData.tradingOutletName,
                receiptNumber = insertData.receiptNumber,
                receiptDate = insertData.receiptDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff"),
                redeemedValue = s,
                receiptLines = insertData.receiptLines,
                glLines = insertData.glLines 
            };

            int returnCode = 0; string ErrorMess = "";
            string requrl = ConstructURI(ICouponApiSettingsEnum.PostSales);
            requrl += insertData.couponRef + "/sales";
            try
            {
                dynamic tokenResponse = wapich.PutInsertReceiptData<dynamic>(requrl, body, header, out returnCode, out ErrorMess);
                if (returnCode > 299)
                {
                    logger.Info("<<<<<<<<<<<                      Could not InsertSalesData: " + insertData.couponRef + " iCoupon  Api : StatusError" + returnCode + "and Error Message: " + ErrorMess + "                       >>>>>>>>>>>>>>>>>>>>");
                    throw new Exception("Invalid iCoupon Request UpdateCoupon: " + insertData.couponRef + "");
                }
                CustomGenericJsonSerializers<Redemption> cjson = new CustomGenericJsonSerializers<Redemption>();
                //CouponsResponse ret = cjson.GenericDeseriallize(tokenResponse);
                return insertData;
            }
            catch (Exception ex) { logger.Error("Exception while Updating Coupons: " + insertData.couponRef + " ex:" + ex.ToString()); throw ex; }
        }
    }
}
