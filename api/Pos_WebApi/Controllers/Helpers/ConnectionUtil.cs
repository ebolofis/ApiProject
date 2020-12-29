using log4net;
using Pos_WebApi.Models;
using System;
using System.Linq;
using System.Web;
using Symposium.Models.Models;
using Symposium.Helpers;

namespace Pos_WebApi.Helpers
{
    public static class ConnectionUtil
    {
        static ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Get Connection String based on StoreId client provides as Parameter or Header
        /// </summary>
        /// <returns></returns>
        public static string ConfigureConnectionString()
        {
            string currentDS = "";
            string currentDb = "";
            string dbUsername = "websa";
            string dbPassword = "qwer!2#4";
            bool isIntegrated = false;
            Guid storeid = Guid.Empty;
            bool storeidfound;
            // find StoreId as Param
            storeidfound = Guid.TryParse(HttpContext.Current.Request.Params["storeid"], out storeid);
            // find StoreId as Header
            if (!storeidfound) storeidfound = Guid.TryParse(HttpContext.Current.Request.Headers["STOREID"], out storeid);
            if (!storeidfound) logger.Debug("STOREID '"+ storeid.ToString()+ "' did not find as Param nor as Header. (it may be part of url)");

                bool forlogin = false;
            bool isforlogin = Boolean.TryParse(HttpContext.Current.Request.Params["forlogin"], out forlogin);
            string xml = System.Web.HttpContext.Current.Server.MapPath("~/") + "Xml\\UsersToDatabases.xml";
            Installations installations = XmlHelpers.ParseXmlDoc(xml);
            if (HttpContext.Current.User.Identity.IsAuthenticated == true && storeidfound)
            {
                var user = installations.Stores.Where(w =>  w.Id == storeid); // Where(w => w.Username == HttpContext.Current.User.Identity.Name && w.Id == storeid)
                currentDb = user.FirstOrDefault().DataBase;
                currentDS = user.FirstOrDefault().DataSource;
                if (user.FirstOrDefault().DataSource != "46.231.115.182,1523")
                {
                    //dbUsername = user.FirstOrDefault().DataBaseUsername;
                    //dbPassword = user.FirstOrDefault().DataBasePassword;
                    isIntegrated = true;
                }
                isIntegrated = user.FirstOrDefault().IsIntegrated != null ? user.FirstOrDefault().IsIntegrated == "true" ? true : false : isIntegrated;
                if (!isIntegrated)
                {
                    dbUsername = user.FirstOrDefault().DataBaseUsername;
                    dbPassword = user.FirstOrDefault().DataBasePassword;
                }
            }
            else if (HttpContext.Current.User.Identity.IsAuthenticated == false && isforlogin == true && forlogin == true)
            {
                var user = installations.Stores.Where(w => w.Id == storeid);
                currentDb = user.FirstOrDefault().DataBase;
                currentDS = user.FirstOrDefault().DataSource;
                if (user.FirstOrDefault().DataSource != "46.231.115.182,1523")
                {
                    //dbUsername = user.FirstOrDefault().DataBaseUsername;
                    //dbPassword = user.FirstOrDefault().DataBasePassword;
                    isIntegrated = true;
                }
                isIntegrated = user.FirstOrDefault().IsIntegrated != null ? user.FirstOrDefault().IsIntegrated == "true" ? true : false : isIntegrated;
                if (!isIntegrated)
                {
                    dbUsername = user.FirstOrDefault().DataBaseUsername;
                    dbPassword = user.FirstOrDefault().DataBasePassword;
                }
            }

            if (!isIntegrated)
            {
                return @"data source=" + currentDS + @";initial catalog=" + currentDb + @";persist security info=True;user id=" + dbUsername + ";password=" + dbPassword + ";MultipleActiveResultSets=True;App=EntityFramework";
            }
            else
            {
                //GIA LOCAL EGKATASTASEIS
                return @"data source=" + currentDS + @";initial catalog=" + currentDb + @";persist security info=True;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            }
        }


        public static string ConfigureConnectionStringByStoreId(Guid storeid)
        {
            string currentDS = "";
            string currentDb = "";
            string dbUsername = "websa";
            string dbPassword = "qwer!2#4";
            bool isIntegrated = false;


            string xml = System.Web.Hosting.HostingEnvironment.MapPath("~/") + "Xml\\UsersToDatabases.xml";

            Installations installations = XmlHelpers.ParseXmlDoc(xml);

            var user = installations.Stores.Where(w => w.Id == storeid);
            if (user == null || user.Count()==0) throw new BusinessException("Invalid StoreId");
            currentDb = user.FirstOrDefault().DataBase;
            currentDS = user.FirstOrDefault().DataSource;
            if (user.FirstOrDefault().DataSource != "46.231.115.182,1523")
            {
                //dbUsername = user.FirstOrDefault().DataBaseUsername;
                //dbPassword = user.FirstOrDefault().DataBasePassword;
                isIntegrated = true;
            }
            isIntegrated = user.FirstOrDefault().IsIntegrated != null ? user.FirstOrDefault().IsIntegrated == "true" ? true : false : isIntegrated;
            if (!isIntegrated)
            {
                dbUsername = user.FirstOrDefault().DataBaseUsername;
                dbPassword = user.FirstOrDefault().DataBasePassword;
            }

            if (!isIntegrated)
            {
                return @"data source=" + currentDS + @";initial catalog=" + currentDb + @";persist security info=True;user id=" + dbUsername + ";password=" + dbPassword + ";MultipleActiveResultSets=True;App=EntityFramework";
            }
            else
            {
                //GIA LOCAL EGKATASTASEIS
                return @"data source=" + currentDS + @";initial catalog=" + currentDb + @";persist security info=True;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            }
        }


    }




}