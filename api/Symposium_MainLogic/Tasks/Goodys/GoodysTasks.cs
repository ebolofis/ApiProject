using log4net;
using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.ExternalSystems;
using Symposium.Plugins;
using Symposium.WebApi.DataAccess.Interfaces.DT.Goodys;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Goodys;
using System;
using System.IO;
using System.Net;


namespace Symposium.WebApi.MainLogic.Tasks.Goodys
{
    public class GoodysTasks : IGoodysTasks
    {
        IGoodysDT GoodysDT;
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GoodysTasks(IGoodysDT GoodysDT)
        {
            this.GoodysDT = GoodysDT;
        }

        //List<Vodafone11Model> GetVodafone11Promos(DBInfoModel dbInfo);
        public OrderModel GetGoodysExternalOrderID(long InvoiceId, DBInfoModel dbInfo)
        {
            return GoodysDT.GetGoodysExternalOrderID(InvoiceId, dbInfo);
        }

        public long GetInvoiceid(DBInfoModel dbinfo, long orderno)
        {
            return GoodysDT.GetInvoiceid(dbinfo, orderno);
        }
        ///api/web/custom/loyalty/cancelOrder?externalOrderId=12345
        /// <summary>
        /// Canceling the ExternalOrderId of a specific Invoice
        /// </summary>
        /// 
       public long GetOpenOrders(DBInfoModel DBInfo)
        {
            return GoodysDT.GetOpenOrders(DBInfo);
        }

        public void CancelGoodysOrderBasedOnExternalOrderId(string ExternalOrderId)
        {
            try { 
            string veltiLoyaltyUrlRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "veltiLoyalty");
            var veltiLoyaltyUrl = veltiLoyaltyUrlRaw.Trim();

            string validateUrl = (veltiLoyaltyUrl + "cancelOrder?" + "externalOrderId=" + ExternalOrderId);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(validateUrl);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream()) // StatusDescription = e.g Response 200, 400, 404, 500 etc.
                using (StreamReader reader = new StreamReader(stream))
                {
                    string status = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                logger.Error("Error while canceling Goodys Goodys Order : " + e.ToString());
            }
        }

        public LoyaltyCoupons GetCouponInfoFromOrder(DA_OrderModel order)
        {
            LoyaltyCoupons loyaltyCouponInfo;
            try
            {
                ExternalObjectModel externalObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ExternalObjectModel>(string.IsNullOrEmpty(order.ExtObj) ? "" : order.ExtObj);
                loyaltyCouponInfo = externalObject.LoyaltyCouponInfo;
            }
            catch (Exception e)
            {
                logger.Error("Error getting coupon info from ExtObj: " + e.ToString());
                loyaltyCouponInfo = null;
            }
            return loyaltyCouponInfo;
        }

        public bool RedeemLoyalty(DBInfoModel dbInfo, DA_OrderModel order)
        {
            bool loyaltyRedeemed = false;
            PluginHelper pluginHelper = new PluginHelper();
            try
            {
                object ImplementedClassInstance = pluginHelper.InstanciatePlugin(typeof(ExternalLoyalty));
                object[] InvokedMethodParameters = { dbInfo, logger, order };
                loyaltyRedeemed = pluginHelper.InvokePluginMethod<bool>(ImplementedClassInstance, "RedeemLoyalty", new[] { typeof(DBInfoModel), typeof(ILog), typeof(DA_OrderModel) }, InvokedMethodParameters);
            }
            catch (Exception e)
            {
                logger.Error("Error calling ExternalLoyalty plugin: " + e.ToString());
                loyaltyRedeemed = false;
            }
            return loyaltyRedeemed;
        }

        public bool CancelLoyalty(DBInfoModel dbInfo, string externalOrderId)
        {
            bool loyaltyCanceled = false;
            PluginHelper pluginHelper = new PluginHelper();
            try
            {
                object ImplementedClassInstance = pluginHelper.InstanciatePlugin(typeof(ExternalLoyalty));
                object[] InvokedMethodParameters = { dbInfo, logger, externalOrderId };
                loyaltyCanceled = pluginHelper.InvokePluginMethod<bool>(ImplementedClassInstance, "CancelLoyalty", new[] { typeof(DBInfoModel), typeof(ILog), typeof(string) }, InvokedMethodParameters);
            }
            catch (Exception e)
            {
                logger.Error("Error calling ExternalLoyalty plugin: " + e.ToString());
                loyaltyCanceled = false;
            }
            return loyaltyCanceled;
        }

        public string GetCouponDetails(DBInfoModel dbInfo, GoodysCouponDetailInfo orderInfo)
        {
            string couponDetails = null;
            PluginHelper pluginHelper = new PluginHelper();
            try
            {
                object ImplementedClassInstance = pluginHelper.InstanciatePlugin(typeof(ExternalLoyalty));
                object[] InvokedMethodParameters = { dbInfo, logger, orderInfo };
                couponDetails = pluginHelper.InvokePluginMethod<string>(ImplementedClassInstance, "GetCouponDetails", new[] { typeof(DBInfoModel), typeof(ILog), typeof(GoodysCouponDetailInfo) }, InvokedMethodParameters);
            }
            catch (Exception e)
            {
                logger.Error("Error calling ExternalLoyalty plugin: " + e.ToString());
                couponDetails = null;
            }
            return couponDetails;
        }

        /// <summary>
        /// Return's a Login responce model
        /// If Param allAddress = true then shipping and billing addresses returned else only shipping addresses.
        /// Deleted addresses never returned
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="AccountId"></param>
        /// <param name="allAddresses"></param>
        /// <returns></returns>
        public GoodysLoginResponceModel GetLoginResponceModel(DBInfoModel dbInfo, string AccountId, bool allAddresses = true)
        {
            GoodysLoginResponceModel res = GoodysDT.GetLoginResponceModel(dbInfo, AccountId);

            if (!allAddresses)
                res.addressList.RemoveAll(r => r.isShipping == false);

            return res;
        }
    }
}



