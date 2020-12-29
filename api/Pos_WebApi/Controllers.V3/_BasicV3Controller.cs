using log4net;
using Pos_WebApi.Helpers;
using Pos_WebApi.Helpers.V3;
using Pos_WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    /// <summary>
    /// Basic ApiController that provides:
    /// <para> Store  for DB connection string creation</para>
    /// <para> Log4net logger</para>
    /// </summary>
    public   class BasicV3Controller : ApiController
    {
        /// <summary>
        /// Store Object
        /// </summary>
        protected Symposium.Models.Models.DBInfoModel DBInfo;
        protected long StaffId;
        protected long CustomerId;
        //  protected Guid StoreId;

        /// <summary>
        /// Log4net logger
        /// </summary>
        protected ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BasicV3Controller()
        {
            //Get Store from Header
            if (HttpContext.Current.Request.HttpMethod != "OPTIONS")
            {
                HttpRequestUtil httpUtil = new HttpRequestUtil();
                DBInfo = httpUtil.GetStore();
                StaffId = httpUtil.GetStaffId();
                CustomerId = httpUtil.GetCustomerId();
            }


        
        }
    }
}
