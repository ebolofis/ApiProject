using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Pos_WebApi.Models;
using log4net;

namespace Pos_WebApi.Helpers
{
    public class CreateTransfer
    {
        public static TranferResultModel CreatePMSTransfer(TransferObject tpms, string storeid)
        {
            ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            string url = tpms.HotelUri + "/Public/CreateTransfer?" + "&hotelid=" + tpms.HotelId + "&resId=" + tpms.resId + "&profileName=" + tpms.profileName + "&description="
                + tpms.description + "&departmentId=" + tpms.departmentId + "&amount=" + tpms.amount + "&roomName=" + tpms.RoomName;// +"&vat=" + tpms.VatPercentage;
            string result = "";
            using (var w = new ExtendedWebClient()) //new WebClient())
            {
                w.Encoding = System.Text.Encoding.UTF8;
               
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                   // var s = w.UploadString(url, string.Empty); // send json with post
                }
                catch (Exception ex) { // error occured in call
                    logger.Error(ex.ToString());
                    return new TranferResultModel(tpms, json_data, ex.Message, storeid);
                }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
               
                int rescode ;

                // error occured in PMS
                if (!Int32.TryParse(json_data, out rescode))
                {
                    return new TranferResultModel(tpms, string.Empty, json_data, storeid);
                }
                else
                {
                    return new TranferResultModel(tpms, json_data, string.Empty, storeid);
                }

                

               
            }
            //return result;
        }
    }


    public static class CreateHelperObjects
    {
        public static TransferObject CreateTransferObject(HotelInfo hotel, bool IsCreditCard, string roomOfCC, TransferToPms tpms, Guid identifier)
        {
            TransferObject to = new TransferObject();
            //
            to.TransferIdentifier = tpms.TransferIdentifier;
            //
            to.HotelId = (int)hotel.HotelId;
            to.amount = (decimal)tpms.Total;
            int PmsDepartmentId = 0;
            var topmsint = int.TryParse(tpms.PmsDepartmentId, out PmsDepartmentId);
            to.departmentId = PmsDepartmentId;
            to.description = tpms.Description;
            to.profileName = tpms.ProfileName;
            int resid = 0;
            var toint = int.TryParse(tpms.RegNo, out resid);
            to.resId = resid;
            to.TransferIdentifier = identifier;
            to.HotelUri = hotel.HotelUri;
            to.RoomName = tpms.RoomDescription;
            if (IsCreditCard)
            {
                to.RoomName = roomOfCC;
            }
            return to;
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

    public class TranferResultModel
    {
        public TranferResultModel(TransferObject obj, string trId, string trError, string storeid) 
        {
            this.TransferObj = obj;
            this.TransferResponseId = trId;
            this.TransferErrorMessage = trError;
            this.StoreId = storeid;
        }

        public TransferObject TransferObj { get; set; }

        public string TransferResponseId { get; set; }

        public string TransferErrorMessage { get; set; }

        public string StoreId { get; set; }

    }

    public class PmsTranferModel
    {
        public int resId { get; set; }

        public string profileName { get; set; }

        public string description { get; set; }

        public int departmentId { get; set; }

        public decimal amount { get; set; }

       
    }
}