using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Pos_WebApi.Modules {


    public class TraceHandler : DelegatingHandler
    {
        log4net.ILog logger;
        public TraceHandler()
        {
            this.logger = log4net.LogManager.GetLogger(this.GetType());
        }
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string debugInfo;
            //1. Request
            if (logger.IsDebugEnabled) { 
                    
                    try
                {
                    if (logger.IsInfoEnabled )
                    {
                        debugInfo = "";
                        string requestBody = await request.Content.ReadAsStringAsync();
                      if (request.RequestUri.LocalPath.Contains("/api/v3/da/Efood/Repost") ||
                            request.RequestUri.LocalPath.Contains("/api/v3/ProductExtras/UpsertProductExtras") ||
                            request.RequestUri.LocalPath.Contains("/api/v3/PriceListDetail/UpsertPricelistDetail") ||
                            request.RequestUri.LocalPath.Contains("/api/v3/Ingredients/UpsertIngredients") ||
                            request.RequestUri.LocalPath.Contains("/api/v3/PageButton/UpsertPageButton") ||
                            request.RequestUri.LocalPath.Contains("/api/v3/Ingredient_ProdCategoryAssoc/UpsertIngredient_ProdCategoryAssoc")
                            )
                        {
                            //Request Body not included
                            debugInfo = string.Format("Request  :  [{0}]  [{1}] [{2}] - {3}", request.Method, DetermineCompName(), request.RequestUri, "<Request Body not included>").Replace("\\r\\n", Environment.NewLine);                   
                        }
                        else
                        {
                            debugInfo = string.Format("Request  :  [{0}]  [{1}] [{2}] - {3}", request.Method, DetermineCompName(), request.RequestUri, requestBody).Replace("\\r\\n", Environment.NewLine);
                        }

                        if (request.RequestUri.LocalPath.Contains("/api/v3/da/Config/ping"))
                            debugInfo = null;
                        if (debugInfo != null && request.RequestUri.LocalPath.Contains("/api/InvoiceForDisplay/Post/") && debugInfo.Length > 800)
                            debugInfo = debugInfo.Substring(0, 799);
                        if (debugInfo!=null && debugInfo.Length > 1000) debugInfo = debugInfo + Environment.NewLine;
                        if (request.Method != HttpMethod.Options)
                            if (debugInfo!=null) logger.Debug(debugInfo); // <--- Info/Debug ---
                            //else
                            //if (debugInfo != null) logger.Debug(debugInfo);

                    }
                }
                catch(Exception ex)
                {
                    logger.Error("Error tracking Request: " + ex.ToString());
                }
            }

            //2.>>> Call the inner handler. <<<<<<
            var response = await base.SendAsync(request, cancellationToken);


            //3. Response 
           
             try
             {
                if (logger.IsDebugEnabled)
                {
                    //3a.
                    if (response.Content != null && logger.IsInfoEnabled && (int)response.StatusCode < 400)  //(response.Content != null && (logger.IsDebugEnabled || logger.IsInfoEnabled) && (int)response.StatusCode < 400)
                    {
                        //flag for not logging long response body for some uri. (only body is not logged, response is logged as well)
                        bool isPageButton = (request.RequestUri.LocalPath == "/api/pagebutton" && request.RequestUri.ToString().Contains("&pricelistid=") && request.RequestUri.ToString().Contains("&posid="))
                                         || (request.RequestUri.LocalPath == "/api/pages" && request.RequestUri.ToString().Contains("&posid="))
                                         || (request.RequestUri.LocalPath == "/api/tablefordisplay" && request.RequestUri.ToString().Contains("&posinfoid="))
                                         || request.RequestUri.LocalPath == "/api/PredefinedCredits"
                                         || (request.RequestUri.LocalPath == "/api/SalesPosLookups" && request.RequestUri.ToString().Contains("&type="))
                                         || (request.RequestUri.LocalPath == "/api/staff" && request.RequestUri.ToString().Contains("&forlogin=") && request.RequestUri.ToString().Contains("&posid=")
                                         || (request.RequestUri.LocalPath.Contains("v3/DeliveryOrders/PagedOrdersByState/"))
                                         || (request.RequestUri.LocalPath.Contains("v3/DeliveryOrders/StateCounts"))
                                         || (request.RequestUri.LocalPath.Contains("/api/PostOrderStatus/"))
                                         || (request.RequestUri.LocalPath.Contains("/api/v3/da/Efood/Repost"))
                                         || (request.RequestUri.LocalPath.Contains("/api/v3/da/Config/ping"))
                                         || (request.RequestUri.LocalPath.Contains("/api/v3/da/Product/getflat")) ||
                                           request.RequestUri.LocalPath.Contains("/api/v3/ProductExtras/UpsertProductExtras") ||
                                           request.RequestUri.LocalPath.Contains("/api/v3/PriceListDetail/UpsertPricelistDetail") ||
                                           request.RequestUri.LocalPath.Contains("/api/v3/Ingredients/UpsertIngredients") ||
                                           request.RequestUri.LocalPath.Contains("/api/v3/PageButton/UpsertPageButton") ||
                                           request.RequestUri.LocalPath.Contains("/api/v3/EndOfDay/Preview/1") ||
                                           request.RequestUri.LocalPath.Contains("/api/InvoiceForDisplay?storeid=")
                                         );

                        if (isPageButton)
                        {
                            ////if (logger.IsDebugEnabled)
                            ////{
                            ////    debugInfo = await getResponseBody(request, response);
                            ////    logger.Debug(debugInfo);
                            ////}
                            ////else
                            ////{
                            debugInfo = string.Format("Response : [{0}] [{1}]  [{2}] - <ResponseBody not included>", request.Method, response.StatusCode, request.RequestUri).Replace("\\r\\n", Environment.NewLine);

                            if (!(request.RequestUri.LocalPath.Contains("/api/v3/da/Config/ping")))
                                logger.Debug(debugInfo); //<--- Info/Debug ---
                                                         ////}

                        }
                        else
                        {
                            //if (request.Method == HttpMethod.Options && logger.IsDebugEnabled)
                            //{
                            //    debugInfo = await getResponseBody(request, response);
                            //    logger.Debug(debugInfo);
                            //}
                            //else 
                            if (request.Method != HttpMethod.Options)
                            {
                                debugInfo = await getResponseBody(request, response);
                                logger.Debug(debugInfo);//<--- Info/Debug ---
                            }
                            // logger.Info(debugInfo);
                        }

                    }
                    

                    //3c.
                    if (response.Content == null && logger.IsInfoEnabled)
                    {
                        //if (request.Method == HttpMethod.Options && logger.IsDebugEnabled)
                        //{
                        //    var warmInfo = string.Format("Response : [{0}] [{1}]  [{2}] --- {3}", request.Method, response.StatusCode, request.RequestUri, response.ToString()).Replace("\\r\\n", Environment.NewLine);
                        //    logger.Debug(warmInfo);
                        //}
                        //else 
                        if (request.Method != HttpMethod.Options)
                        {
                            var warmInfo = string.Format("Response : [{0}] [{1}]  [{2}] --- {3}", request.Method, response.StatusCode, request.RequestUri, response.ToString()).Replace("\\r\\n", Environment.NewLine);
                            logger.Debug(warmInfo); //<--- Info/Debug ---
                        }

                    }
                }
                //3b.
                if ((int)response.StatusCode >= 400 && response.Content != null )
                {
                    
                    var s = await getResponseBody(request, response);
                    if (!s.Contains("Symposium.Helpers.BusinessException"))
                    {
                        debugInfo = string.Format("Response : [{0}] [{1}]  [{2}]  -- ", request.Method, response.StatusCode, request.RequestUri);
                        logger.Error(debugInfo);

                    }
                }

            }
                catch (Exception ex)
                {
                    logger.Error("Error tracking Response: " + ex.ToString());
                }
            
            return response;
        }

        private async Task<string> getResponseBody(HttpRequestMessage request, HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            string debugInfo = string.Format("Response : [{0}] [{1}]  [{2}] - {3}", request.Method, response.StatusCode, request.RequestUri, responseBody).Replace("\\r\\n", Environment.NewLine);
            if (debugInfo.Length > 1000) debugInfo = debugInfo + Environment.NewLine;

            return debugInfo;
        }

        private  string getRequestBody(HttpRequestMessage request, HttpResponseMessage response)
        {
            string requestBody =  request.Content.ReadAsStringAsync().Result;
          if(requestBody!=null)  requestBody = requestBody.Replace("\\r\\n", Environment.NewLine);
            return requestBody;
        }

        public void Debug(string message)
        {
            ThreadPool.QueueUserWorkItem(task => logger.Debug(message));
        }

        public void Info(string message)
        {
            ThreadPool.QueueUserWorkItem(task => logger.Info(message));
        }
        public void Error(string message)
        {
            ThreadPool.QueueUserWorkItem(task => logger.Info(message));
        }


        /// <summary>
        /// Get client's computer name
        /// </summary>
        /// <returns></returns>
        public static string DetermineCompName()
        {
            try
            {
                string ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";
                //IPAddress myIP = IPAddress.Parse(ip);
                //IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
                //List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
                //return compName.First();
                return ip;
            }
            catch(Exception ex)
            {
                return "ClientNameNotFound";
            }
           
        }
    }



    ////public class TraceHandler : DelegatingHandler {
    ////    log4net.ILog logger;
    ////    public TraceHandler( ) {
    ////        this.logger = log4net.LogManager.GetLogger(this.GetType());
    ////    }
    ////    protected async override Task<HttpResponseMessage> SendAsync(
    ////        HttpRequestMessage request, CancellationToken cancellationToken ) {
    ////        var debugInfo = string.Format("request uri --> [{0}] {1}", request.Method, request.RequestUri);
    ////        Debug.WriteLine(debugInfo);
    ////        logger.Debug(debugInfo);

    ////        // Call the inner handler.
    ////        var response = await base.SendAsync(request, cancellationToken);
    ////        return response;
    ////    }
    ////}
}