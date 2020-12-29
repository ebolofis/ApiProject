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
using System.Linq;
using log4net;
using Symposium.Helpers;
using Symposium.Models.Models.ExternalSystems;

namespace Pos_WebApi.Controllers.Helpers
{
    public class goodysLoyaltyController : ApiController
    {
        private string xmlFilePath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Web.config";
        string veltiLoyaltyUrl = string.Empty;
        private string gstoreid = string.Empty;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public goodysLoyaltyController()
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
            string veltiLoyaltyUrlRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "veltiLoyalty");
            veltiLoyaltyUrl = veltiLoyaltyUrlRaw.Trim();
            string gstoreidRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "goodysStoreId");
            gstoreid = gstoreidRaw.Trim();
        }

        /// <summary>
        /// Get loyalty member using loyaltyId
        /// </summary>
        /// <param name="email"></param>
        /// <param name="loyaltyId"></param>
        /// <returns></returns>
        [Route("api/getLoyaltyMember/{email}/{loyaltyId}")]
        [HttpGet]
        public string getLoyaltyMember(string email, string loyaltyId)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            string validateUrl = email != "null" ? (veltiLoyaltyUrl + "getLoyaltyMember" + "?email=" + email + "&loyaltyId=" + loyaltyId + "&channel=store&store=" + gstoreid) : (veltiLoyaltyUrl + "getLoyaltyMember" + "?&loyaltyId=" + loyaltyId + "&channel=store&store=" + gstoreid);

            using (var w = new ExtendedWebClient()) //new WebClient())
            {
                w.Encoding = System.Text.Encoding.UTF8;

                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    logger.Info("Sending : " + validateUrl);
                    json_data = w.DownloadString(validateUrl);
                    logger.Info("Response : " + json_data);
                }
                catch (Exception ex)
                { // error occured in call
                    logger.Error(ex.ToString());
                    json_data = ex.Message;
                }
                return json_data;
            }
        }
        /// <summary>
        /// Resending SMS to New Goodys Customers which have status Pending External
        /// </summary>
        /// 
        [Route("api/ResendSMStoPendingExternalCustomers/{loyaltyId?}")]
        [HttpPost]
        public string ResendSMStoPendingExternalCustomers(string loyaltyId)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            string veltiLoyaltyUrlRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "veltiLoyalty");
            veltiLoyaltyUrl = veltiLoyaltyUrlRaw.Trim();
            string temp = loyaltyId;
            var obj = new { msisdn = temp };
            var json = JsonConvert.SerializeObject(obj, Formatting.None,
 new JsonSerializerSettings
 {
     NullValueHandling = NullValueHandling.Ignore
 });
            string validateUrl = (veltiLoyaltyUrl + "resendSmsToPendingExternal");

            var postData = json;

            var request = (HttpWebRequest)WebRequest.Create(validateUrl);
            var data = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = data.Length;
            request.ContentType = "application/json";
            request.Method = "POST";

            using (var stream = request.GetRequestStream())
            {
                try
                {
                    stream.Write(data, 0, data.Length);
                }
                catch (InvalidCastException e)
                {
                    logger.Error(e.ToString());
                }
                return json;
            }

        }
        /// 
        /// <summary>
        /// Updating status for a Goodys Loyalty Customer from Inactive to Pending External
        /// </summary>
        [Route("api/UpdateStatusOfInactiveRegisteredCustomer/{loyaltyId?}")]
        [HttpPost]
        public string UpdateStatusToPendingExternal(string loyaltyId)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            //goodysStoreId
            string veltiLoyaltyUrlRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "veltiLoyalty");
            veltiLoyaltyUrl = veltiLoyaltyUrlRaw.Trim();
            string gstoreidRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "goodysStoreId");
            gstoreid = gstoreidRaw.Trim();

            string temp = loyaltyId;
            var obj = new { email = "", loyaltyId = temp, name = "", surname = "" };
            var json = JsonConvert.SerializeObject(obj, Formatting.None,
             new JsonSerializerSettings
             {
                 NullValueHandling = NullValueHandling.Ignore
             });
            string validateUrl = (veltiLoyaltyUrl + "registerLoyaltyMember?" + "registrationChannel=STORE&store=" + gstoreid);

            var postData = json;

            var request = (HttpWebRequest)WebRequest.Create(validateUrl);
            var data = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = data.Length;
            request.ContentType = "application/json";
            request.Method = "POST";

            using (var stream = request.GetRequestStream())
            {
                try
                {
                    stream.Write(data, 0, data.Length);
                }
                catch (InvalidCastException e)
                {
                    logger.Error(e.ToString());
                }
                return json;
            }
        }
        /// <summary>
        /// Send a request to the Velti api and returns requestResult to client
        /// </summary>
        /// <param name="jsonToSend"></param>
        /// <param name="storeId"></param>
        /// <param name="loyaltyId"></param>
        /// <returns></returns>
        [Route("api/redeemLoyalty/{storeId}/{loyaltyId?}")]
        [HttpPost]
        public string getCouponDetails(GoodysLoyaltyJSON jsonToSend, Guid storeId, string loyaltyId = "")
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            PosEntities db = new PosEntities(false, storeId);

            string posid = jsonToSend.PosId.ToString();
            string ReceiptString = jsonToSend.InvoiceId.ToString().PadLeft(6, '0').Substring(3, 3);
            string LoyaltyIdString = loyaltyId != "" ? loyaltyId.Substring(5, 5) : loyaltyId;
            String sDate = DateTime.Now.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));
            String dy = datevalue.Day.ToString();
            String mn = datevalue.Month.ToString();
            String yy = datevalue.Year.ToString().Substring(2, 2);
            if (mn.Length == 1)
                mn = '0' + mn;

            try
            {

                jsonToSend.store = gstoreid;
                jsonToSend.msisdn = loyaltyId;
                if (jsonToSend.couponCode == null)
                    jsonToSend.couponCode = "";
                if (loyaltyId.Equals(""))
                {
                    jsonToSend.basketValue = null;
                    jsonToSend.orderLines = null;
                    jsonToSend.msisdn = null;
                }
                jsonToSend.externalOrderId = dy + mn + yy + posid + ReceiptString + LoyaltyIdString;
                long invId = jsonToSend.InvoiceId;

                var tempOrder =
                            from a in db.Order
                            join b in db.OrderDetail on a.Id equals b.OrderId
                            join c in db.OrderDetailInvoices on b.Id equals c.OrderDetailId
                            join d in db.Invoices on c.InvoicesId equals d.Id
                            where d.Id == invId
                            select a.Id;

                IEnumerable<Order> tempExtObj =
                    from k in db.Order
                    where k.Id == tempOrder.FirstOrDefault()
                    select (k);

                tempExtObj.FirstOrDefault().ExtObj = jsonToSend.externalOrderId;

                //db.Entry(obj).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (Exception exc)
                {
                    logger.Error(exc.ToString());
                }

                //var ExtObj = db.Order(xmlFilePath=)
                //var ExtObj = db.Order.Select(f => f.OID == OID).FirstOrDefault();
                //foo.ID = newID;
                //database.SubmitChanges();


                //var ExtObj = db.Order.Select(r => r.ExtObj)
                //       .Where(c => c.Name.ToLower().Equals("id"));
                //foreach (var col in cols)
                //{
                //    col.IsUpdated = true;
                //}


                var request = (HttpWebRequest)WebRequest.Create(veltiLoyaltyUrl + "redeem");
                var json = JsonConvert.SerializeObject(jsonToSend, Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                logger.Info("Request Data : " + json.ToString());
                var postData = json;

                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                Loyalty loyaltyEnt = new Loyalty();
                List<Loyalty> loyaltyToAdd = new List<Loyalty>();

                logger.Info("Response Status : " + response.StatusCode.ToString());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (jsonToSend.giftCardCodes != null)
                    {
                        for (int i = 0; i < jsonToSend.giftCardCodes.Count(); i++)
                        {
                            StringBuilder tempStr = new StringBuilder();
                            tempStr.Clear();
                            loyaltyEnt.Day = DateTime.Now;
                            loyaltyEnt.LoyalltyId = loyaltyId;
                            loyaltyEnt.CouponCode = jsonToSend.couponCode;

                            tempStr.Append(jsonToSend.giftCardCodes[i] + ", ");

                            loyaltyEnt.GiftcardCode = tempStr.Remove(tempStr.Length - 1, 0).ToString();
                            loyaltyEnt.CouponType = jsonToSend.couponType != null ? jsonToSend.couponType : "";
                            loyaltyEnt.Campaign = jsonToSend.campaign != null ? jsonToSend.campaign : "";
                            loyaltyEnt.Channel = jsonToSend.channel != null ? jsonToSend.channel : "";
                            loyaltyEnt.GiftCardCampaign = jsonToSend.giftCardCampaign[i];
                            loyaltyEnt.GiftCardCouponType = jsonToSend.giftCardTypes[i];
                            loyaltyEnt.InvoicesId = jsonToSend.InvoiceId != null ? jsonToSend.InvoiceId : -1;
                            // loyaltyToAdd.Add(loyaltyEnt);
                            if (logger.IsInfoEnabled) logger.Info("Adding Loyalty [" + JsonConvert.SerializeObject(loyaltyEnt) + "] to DB...");
                            db.Loyalty.Add(loyaltyEnt);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception exc)
                            {
                                logger.Error(exc.ToString());
                            }
                        }

                    }
                    else
                    {
                        StringBuilder tempStr = new StringBuilder();
                        tempStr.Clear();
                        loyaltyEnt.Day = DateTime.Now;
                        loyaltyEnt.LoyalltyId = loyaltyId;
                        loyaltyEnt.CouponCode = jsonToSend.couponCode;

                        loyaltyEnt.GiftcardCode = "";
                        loyaltyEnt.CouponType = jsonToSend.couponType != null ? jsonToSend.couponType : "";
                        loyaltyEnt.Campaign = jsonToSend.campaign != null ? jsonToSend.campaign : "";
                        loyaltyEnt.Channel = jsonToSend.channel != null ? jsonToSend.channel : "";
                        loyaltyEnt.GiftCardCampaign = "";
                        loyaltyEnt.GiftCardCouponType = "";
                        loyaltyEnt.InvoicesId = jsonToSend.InvoiceId != null ? jsonToSend.InvoiceId : -1;
                        // loyaltyToAdd.Add(loyaltyEnt);
                        if (logger.IsInfoEnabled) logger.Info("Adding Loyalty [" + JsonConvert.SerializeObject(loyaltyEnt) + "] to DB...");
                        db.Loyalty.Add(loyaltyEnt);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception exc)
                        {
                            logger.Error(exc.ToString());
                        }
                    }
                }
                else
                {
                    loyaltyEnt.ErrorDescription = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString();
                    if (jsonToSend.giftCardCodes != null)
                    {
                        for (int i = 0; i < jsonToSend.giftCardCodes.Count(); i++)
                        {
                            StringBuilder tempStr = new StringBuilder();
                            tempStr.Clear();
                            loyaltyEnt.Day = DateTime.Now;
                            loyaltyEnt.LoyalltyId = loyaltyId;
                            loyaltyEnt.CouponCode = jsonToSend.couponCode;

                            tempStr.Append(jsonToSend.giftCardCodes[i] + ", ");

                            loyaltyEnt.GiftcardCode = tempStr.Remove(tempStr.Length - 1, 0).ToString();
                            loyaltyEnt.CouponType = jsonToSend.couponType != null ? jsonToSend.couponType : "";
                            loyaltyEnt.Campaign = jsonToSend.campaign != null ? jsonToSend.campaign : "";
                            loyaltyEnt.Channel = jsonToSend.channel != null ? jsonToSend.channel : "";
                            loyaltyEnt.GiftCardCampaign = jsonToSend.giftCardCampaign[i];
                            loyaltyEnt.GiftCardCouponType = jsonToSend.giftCardTypes[i];
                            loyaltyEnt.InvoicesId = jsonToSend.InvoiceId != null ? jsonToSend.InvoiceId : -1;
                            // loyaltyToAdd.Add(loyaltyEnt);
                            if (logger.IsInfoEnabled) logger.Info("Adding Loyalty [" + JsonConvert.SerializeObject(loyaltyEnt) + "] to DB...");
                            db.Loyalty.Add(loyaltyEnt);
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception exc)
                            {
                                logger.Error(exc.ToString());
                            }
                        }

                    }
                    else
                    {
                        StringBuilder tempStr = new StringBuilder();
                        tempStr.Clear();
                        loyaltyEnt.Day = DateTime.Now;
                        loyaltyEnt.LoyalltyId = loyaltyId;
                        loyaltyEnt.CouponCode = jsonToSend.couponCode;

                        loyaltyEnt.GiftcardCode = "";
                        loyaltyEnt.CouponType = jsonToSend.couponType != null ? jsonToSend.couponType : "";
                        loyaltyEnt.Campaign = jsonToSend.campaign != null ? jsonToSend.campaign : "";
                        loyaltyEnt.Channel = jsonToSend.channel != null ? jsonToSend.channel : "";
                        loyaltyEnt.GiftCardCampaign = "";
                        loyaltyEnt.GiftCardCouponType = "";
                        loyaltyEnt.InvoicesId = jsonToSend.InvoiceId != null ? jsonToSend.InvoiceId : -1;
                        // loyaltyToAdd.Add(loyaltyEnt);
                        if (logger.IsInfoEnabled) logger.Info("Adding Loyalty [" + JsonConvert.SerializeObject(loyaltyEnt) + "] to DB...");
                        db.Loyalty.Add(loyaltyEnt);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception exc)
                        {
                            logger.Error(exc.ToString());
                        }
                    }


                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception exc)
                    {
                        logger.Error(exc.ToString());
                    }
                }

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                logger.Info("Response String : " + responseString.ToString());
                return responseString.ToString();

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return ex.Message.ToString();
            }


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