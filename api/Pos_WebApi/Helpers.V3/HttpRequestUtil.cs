using log4net;
using System;
using System.Web;
using Symposium.Helpers.Classes;

namespace Pos_WebApi.Helpers.V3
{
    /// <summary>
    /// Class that get data from the HTTP Request, ex: get proper headers
    /// </summary>
    public class HttpRequestUtil
    {

        /// <summary>
        /// Get Store that BasicAuthHttpModule provides as Header with name STORE
        /// </summary>
        /// <returns>Store</returns>
        public Symposium.Models.Models.DBInfoModel GetStore()
        {
            string storeJson ="";

            //1. find Store as Header
            storeJson = HttpContext.Current.Request.Headers["STORE"];
           
            //2. Log if store did not find.
            if (storeJson ==null || storeJson=="")
            {
                ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                string errStr = "STORE  DID NOT FIND as Header !!!";
                logger.Warn(errStr);
                return null;
            }

            //3. get store
            CustomJsonDeserializers js = new CustomJsonDeserializers();
            Symposium.Models.Models.DBInfoModel store=null;
            try
            {
                store= js.JsonToStore(storeJson);
            }catch(Exception ex)
            {
                ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                string errStr = "Header STORE was Wrong: "+ex.Message;
                logger.Error(ex.ToString());
                return null;
            }
            return store;
        }

        /// <summary>
        /// Get StaffId that BasicAuthHttpModule provides as Header with name STAFFID
        /// </summary>
        /// <returns>Store</returns>
        public long GetStaffId()
        {
            string staffId = "";
            long StaffId = 0;

            //1. find staffId as Header
            staffId = HttpContext.Current.Request.Headers["STAFFID"];

            //2. Log if staffId did not find.
            if (staffId == null || staffId == "")
            {
               // ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
               // string errStr = "staffId  DID NOT FIND as Header !!!";
               // logger.Warn(errStr);
                return 0;
            }

            //3. get long StaffId
            StaffId = Convert.ToInt64(staffId);

            return StaffId;
        }

        /// <summary>
        /// Get StaffId that BasicAuthHttpModule provides as Header with name STAFFID
        /// </summary>
        /// <returns>Store</returns>
        public long GetCustomerId()
        {
            string custId = "";
            long CustId = 0;

            //1. find custId as Header
            custId = HttpContext.Current.Request.Headers["CUSTOMERID"];

            //2. Log if custId did not find.
            if (custId == null || custId == "")
            {
                //ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                //string errStr = "custId  DID NOT FIND as Header !!!";
                //logger.Warn(errStr);
                return 0;
            }

            //3. get long StaffId
            CustId = Convert.ToInt64(custId);

            return CustId;
        }

        /// <summary>
        /// Get StoreId that client provides as Header with name STOREID into his http request (NOT USED)
        /// </summary>
        /// <returns>storeId as Guid</returns>
        public Guid GetStoreId()
        {
            bool storeidfound;
            Guid storeid = Guid.Empty;
            
            //1. find StoreId as Header
            storeidfound = storeidfound = Guid.TryParse(HttpContext.Current.Request.Headers["STOREID"], out storeid);
            // find StoreId as Param
            //  if (!storeidfound) Guid.TryParse(HttpContext.Current.Request.Params["storeid"], out storeid);  

            //2. Log if storeid did not find.
            if (!storeidfound)
            {
                ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                string errStr = "STOREID '" + storeid.ToString() + "' DID NOT FIND as Header into client's request !!!";
                logger.Warn(errStr);
            }

            return storeid;
        } 

       



    }
}