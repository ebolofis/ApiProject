using Pos_WebApi.Models;
using Symposium.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace Pos_WebApi.Controllers
{
    [AllowAnonymous]

    public class HomeController : Controller
    {
        /// <summary>
        /// First Page....
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
  
       //     FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            ViewBag.Version = assembly.FullName.Split(',')[1];

            ViewBag.Mode = "";

            bool isDeliveryAgent = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "IsDeliveryAgent");
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");

            if (isDeliveryStore)
                ViewBag.Mode = "as DA Client";

            if (isDeliveryAgent)
                ViewBag.Mode = "as Delivery Agent";


            decimal toso = DeVat(23.0000m, 14.20m);
            decimal nero = DeVat(13.0000m, 41.65m);
            return View();
        }

        public ActionResult WebConfig()
        {
            List<WebConfig> rslt = new List<WebConfig>();
            foreach (string key in ConfigurationManager.AppSettings)
            {
                WebConfig tmp = new WebConfig();
                tmp.key = key;
                tmp.value = ConfigurationManager.AppSettings[key];
                if(tmp.key != "webpages:Version" && tmp.key != "webpages:Enabled" && tmp.key != "PreserveLoginUrl" && tmp.key != "ClientValidationEnabled" && tmp.key != "UnobtrusiveJavaScriptEnabled")
                    rslt.Add(tmp);
            }
            return View(rslt);
        }

        public HttpResponseMessage Save(List<WebConfig> configs)
        {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            foreach (WebConfig item in configs)
            {
                try
                {
                    string str = ConfigurationManager.AppSettings[item.key];
                    if (!String.IsNullOrEmpty(str))
                    {
                        config.AppSettings.Settings[item.key].Value = item.value;
                    }
                    else { config.AppSettings.Settings.Add(item.key, item.value); }
                    config.Save();
                }
                catch (Exception ex)
                {                   
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = ex.Message };
                }
            }

            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        private static decimal DeVat(decimal perc, decimal tempnetbyvat)
        {
            return Math.Round(Math.Round((decimal)(tempnetbyvat / (decimal)(1 + (decimal)(perc / 100))), 3, MidpointRounding.AwayFromZero), 2);
           // return Math.Round((Math.Truncate(10000 * (decimal)(tempnetbyvat / (decimal)(1 + (decimal)(perc / 100)))) / 10000),2,MidpointRounding.ToEven);
        }

        [AllowAnonymous]
        public string CheckWeb()
        {
            return "";
        }

        [AllowAnonymous]
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }
    }
}
