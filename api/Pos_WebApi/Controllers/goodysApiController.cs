using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Collections.Generic;
using System.Web.Configuration;
using log4net;
using Symposium.Helpers;
using Symposium.Models.Models.ExternalSystems;

namespace Pos_WebApi.Controllers.Helpers
{
    public class goodysApiController : ApiController
    {
        private PosEntities db = new PosEntities(false);
        private string xmlFilePath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Web.config";
        string veltiApiUrl = string.Empty;
        private string gstoreid = string.Empty;
        private string gdUrl = string.Empty;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);



        public goodysApiController()
        {
            try
            {
                GetValues();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

        }

        public void GetValues()
        {
            string veltiApiUrlRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "veltiApi");
            veltiApiUrl = veltiApiUrlRaw.Trim();
            string gstoreidRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "goodysStoreId");
            gstoreid = gstoreidRaw.Trim();
            string gdUrlRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "callMobile");
            gdUrl = gdUrlRaw.Trim();
        }

        [Route("api/validateCoupon/{couponCode}/{type}/{msisdn}")]
        [HttpGet]
        public string validateCoupon(string couponCode, int type, string msisdn)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            string validateUrl = string.Empty;

            if (type == 0)
            {
                validateUrl = veltiApiUrl + "validate?couponCode=" + couponCode + "&store=" + gstoreid + "&channel=store";
            }
            if (type == 1)
            {
                validateUrl = veltiApiUrl + "validate?&giftCardCodes=" + couponCode + "&store=" + gstoreid + "&channel=store" + "&msisdn=" + msisdn;
            }
            using (var w = new ExtendedWebClient()) //new WebClient())
            {
                w.Encoding = System.Text.Encoding.UTF8;
                logger.Info("URL to send to Velti for validateCoupon: " + validateUrl);
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {

                    json_data = w.DownloadString(validateUrl);
                }
                catch (Exception ex)
                { // error occured in call
                    logger.Error(ex.ToString());
                    json_data = ex.Message;
                }
                return json_data;
            }

        }


        [Route("api/getCouponDetails/")]
        [HttpPost]
        public string getCouponDetails(GoodysJSON jsonToSend)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            try
            {
                jsonToSend.store = gstoreid;
                var request = (HttpWebRequest)WebRequest.Create(veltiApiUrl + "details");
                var json = JsonConvert.SerializeObject(jsonToSend);
                var postData = json;

                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                logger.Info("URL to send to Velti for getCouponDetails: " + veltiApiUrl + "details");
                logger.Info("The JSON that we send is : " + postData.ToString());
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString.ToString();

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return ex.Message.ToString();
            }


        }

        [Route("api/redeemCoupon/{couponCode}")]
        [HttpGet]
        public string redeemCoupon(string couponCode)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            try
            {
                string validateUrl = veltiApiUrl + "redeem?store=" + gstoreid + "&channel=store&couponCode=" + couponCode;

                string result = "";
                using (var w = new ExtendedWebClient()) //new WebClient())
                {
                    w.Encoding = System.Text.Encoding.UTF8;

                    var json_data = string.Empty;
                    // attempt to download JSON data as a string
                    try
                    {
                        logger.Info("URL to send to Velti for redeemCoupon: " + validateUrl);
                        json_data = w.DownloadString(validateUrl);
                        // var s = w.UploadString(url, string.Empty); // send json with post
                    }
                    catch (Exception ex)
                    { // error occured in call
                        logger.Error(ex.ToString());
                        return result;
                    }
                    return json_data;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return ex.Message;
            }

        }

        //Notification for Order Ready
        [Route("api/notifyCustomerTableForPickUp/{OrderId}")]
        [HttpGet]
        public string notifyCustomerTableForPickUp(string OrderId)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            string validateUrl = gdUrl + OrderId;
            string result = string.Empty;
            using (var w = new ExtendedWebClient())
            {
                w.Encoding = System.Text.Encoding.UTF8;
                try
                {
                    logger.Info("URL to send to ATCOM: " + validateUrl);
                    result = w.DownloadString(validateUrl);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    result = ex.Message;
                }

            }

            return result;
        }

        public class ExtendedWebClient : WebClient
        {

            private int timeout;
            public int Timeout
            {
                get
                {
                    return timeout;
                }
                set
                {
                    timeout = value;
                }
            }

            public ExtendedWebClient()
            {
                this.timeout = 60000;//In Milli seconds

            }

            public ExtendedWebClient(int timeout)
            {
                this.timeout = timeout;//In Milli seconds

            }

            public ExtendedWebClient(Uri address)
            {
                this.timeout = 60000;//In Milli seconds
                var objWebClient = GetWebRequest(address);
            }
            protected override WebRequest GetWebRequest(Uri address)
            {
                var objWebRequest = base.GetWebRequest(address);
                objWebRequest.Timeout = this.timeout;
                return objWebRequest;
            }
        }


    }
}