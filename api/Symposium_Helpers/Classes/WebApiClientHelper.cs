using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Xml.Serialization;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models.ExternalSystems;
using System.Net;

namespace Symposium.Helpers.Classes
{
    /// <summary>
    /// Rest Client
    /// </summary>
    public class WebApiClientHelper : IWebApiClientHelper
    {

        /// <summary>
        /// Get Request
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="user">user and password for Authentication Header. Format for Basic: "Username:Password", Format for OAuth2: "Bearer  ZTdmZmY1Zjc5MTQ4NDQ5ZTEzMzIyZTOQ"</param>
        /// <param name="headers">custom headers (key: header name, value: header value)</param>
        /// <param name="returnCode">return Code (200, 400,...)</param>
        /// <param name="ErrorMess">Error message (for 200 ErrorMess="")</param>
        /// <param name="mediaType">mediaType. Default: "application/json" . Other Values:  application/xml  </param>
        /// <param name="authenticationType">Type of authentication (Basic or OAuth2) </param>
        /// <returns>the result or the request as string </returns>
        public string GetRequest(string url, string user, Dictionary<string, string> headers, out int returnCode, out string ErrorMess, string mediaType = "application/json", string authenticationType = "Basic")
        {
            string result = null;
            HttpRequestMessage request;
            request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Method = HttpMethod.Get;
            // HttpClient client = new HttpClient();
            lock (request)
            {
                using (HttpClient client = new HttpClient())
                {
                    //1. Create  headers
                    setHeaders(client, authenticationType, user, headers);

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    
                    //3. Send Request
                    using (HttpResponseMessage response = client.SendAsync(request).Result)
                    {
                        

                        var readAsStringAsync = response.Content.ReadAsStringAsync();
                        returnCode = response.StatusCode.GetHashCode();
                        if (returnCode == 200)
                        {
                            ErrorMess = "";
                            result = readAsStringAsync.Result;
                        }
                        else
                        {
                            if (readAsStringAsync.Result is HttpMessage)
                            {
                                //if(mediaType == "application/json")
                                ErrorMess = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpMessage>(readAsStringAsync.Result).Message;
                                //else
                                //{
                                //    XmlSerializer serializer = new XmlSerializer(typeof(HttpMessage));
                                //    serializer.Deserialize(readAsStringAsync.Result)
                                //}
                            }

                            else
                                ErrorMess = readAsStringAsync.Result;
                        }
                    }
                }

            }
            request.Dispose();
            return result;
        }


        /// <summary>
        /// Post Request
        /// </summary>
        /// <param name="model">model to Post</param>
        /// <param name="url">url</param>
        /// <param name="user">user and password for Authentication Header. Format "Username + : + Password"</param>
        /// <param name="headers">custom headers (key: header name, value: header value)</param>
        /// <param name="returnCode">return Code (200, 400,...)</param>
        /// <param name="ErrorMess">Error message (for 200 ErrorMess="")</param>
        /// <param name="mediaType">mediaType. Default: "application/json". Other Values: application/xml </param>
        /// <param name="authenticationType">Type of authentication (Basic or OAuth2) </param>
        /// <returns>the result or the request as string </returns>
        public string PostRequest<T>(T model, string url, string user, Dictionary<string, string> headers, out int returnCode, out string ErrorMess, string mediaType = "application/json", string authenticationType = "Basic")
        {
            MediaTypeFormatter formatter;
            switch (mediaType)
            {
                case "application/json": formatter = new JsonMediaTypeFormatter(); break;
                default: formatter = new XmlMediaTypeFormatter(); break;
            }

            string result = null;
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = new ObjectContent<T>(model, formatter)
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            using (HttpClient client = new HttpClient())
            {
                //1. Create headers
                setHeaders(client, authenticationType, user, headers);

                //2. Send Request
                using (HttpResponseMessage response = client.SendAsync(request).Result)
                {
                    var readAsStringAsync = response.Content.ReadAsStringAsync();
                    returnCode = response.StatusCode.GetHashCode();

                    if (returnCode >= 200 && returnCode <= 299)
                    {
                        ErrorMess = ""; result = readAsStringAsync.Result;
                    }
                    else
                    {
                        if (readAsStringAsync.Result is HttpMessage)
                        {
                            ErrorMess = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpMessage>(readAsStringAsync.Result).Message;
                        }
                        else
                        {
                            ErrorMess = readAsStringAsync.Result;
                        }
                    }
                }
            }
            request.Dispose();
            return result;

        }



        /// <summary>
        /// Patch Request
        /// </summary>
        /// <param name="model">model to Patch</param>
        /// <param name="url">url</param>
        /// <param name="user">user and password for Authentication Header. Format "Username + : + Password"</param>
        /// <param name="headers">custom headers (key: header name, value: header value)</param>
        /// <param name="returnCode">return Code (200, 400,...)</param>
        /// <param name="ErrorMess">Error message (for 200 ErrorMess="")</param>
        /// <param name="mediaType">mediaType. Default: "application/json". Other Values: application/xml </param>
        /// <param name="authenticationType">Type of authentication (Basic or OAuth2) </param>
        /// <returns>the result or the request as string </returns>
        public string PatchRequest<T>(T model, string url, string user, Dictionary<string, string> headers, out int returnCode, out string ErrorMess, string mediaType = "application/json", string authenticationType = "Basic")
        {
            string result = null;
            HttpRequestMessage request;
            request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Method = new HttpMethod("PATCH");

            MediaTypeFormatter formatter;
            if (mediaType == "application/json")
                formatter = new JsonMediaTypeFormatter();
            else
                formatter = new XmlMediaTypeFormatter();

            request.Content = new ObjectContent<T>(model, formatter);

            using (HttpClient client = new HttpClient())
            {
                //1. Create headers
                setHeaders(client, authenticationType, user, headers);

                //2. Send Request
                using (HttpResponseMessage response = client.SendAsync(request).Result)
                {
                    var readAsStringAsync = response.Content.ReadAsStringAsync();
                    returnCode = response.StatusCode.GetHashCode();
                    if (returnCode >= 200 && returnCode <= 299)
                    {
                        ErrorMess = "";
                        result = readAsStringAsync.Result;
                    }
                    else
                    {
                        if (readAsStringAsync.Result is HttpMessage)
                            ErrorMess = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpMessage>(readAsStringAsync.Result).Message;
                        else
                            ErrorMess = readAsStringAsync.Result;
                    }
                }
            }
            request.Dispose();
            return result;

        }




        /// <summary>
        /// Create Headers for the rest client
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="authenticationType">type of authentication (Basic or OAuth2)</param>
        /// <param name="user">user and password or token for Authentication Header. Format for Basic: "Username:Password", Format for OAuth2: "Bearer  ZTdmZmY1Zjc5MTQ4NDQ5ZTEzMzIyZTOQ"</param>
        /// <param name="headers">custom headers </param>
        private void setHeaders(HttpClient client, string authenticationType, string user, Dictionary<string, string> headers)
        {
            //1. Create Authorization header
            if (!string.IsNullOrEmpty(user))
            {
                switch (authenticationType)
                {
                    case "Basic":
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(user)));
                        break;
                    case "OAuth2":
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", user);
                        break;
                }
            }

            //2. Create custom headers
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    if (key != null && headers[key] != null)
                        client.DefaultRequestHeaders.Add(key, headers[key]);
                }
            }
        }


        private class HttpMessage
        {
            public string Message { get; set; }
        }


        public string PostFormUrlEncoded<TResult>(string url, IEnumerable<KeyValuePair<string, string>> postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess)
        {
            try
            {

                string result = null;
                using (HttpClient httpClient = new HttpClient())
                {
                    //1. Create headers
                    //2. Send Request
                    using (var content = new FormUrlEncodedContent(postData))
                    {
                        //content.Headers.Clear();
                        if (heads != null)
                        {
                            foreach (var kvp in heads.ToArray())
                            {
                                if (kvp.Key != null && kvp.Value != null)
                                {
                                    if (kvp.Key.Equals("Authorization"))
                                    {
                                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", kvp.Value);
                                    }
                                    else
                                    {
                                        content.Headers.ContentType = new MediaTypeHeaderValue(kvp.Value.ToString());
                                    }

                                }
                            }
                        }

                        using (HttpResponseMessage res = httpClient.PostAsync(url, content).Result)
                        {
                            var readAsStringAsync = res.Content.ReadAsStringAsync();
                            returnCode = res.StatusCode.GetHashCode();

                            if (returnCode >= 200 && returnCode <= 299)
                            {
                                ErrorMess = ""; result = readAsStringAsync.Result;
                            }
                            else
                            {
                                if (readAsStringAsync.Result is HttpMessage)
                                {
                                    ErrorMess = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpMessage>(readAsStringAsync.Result).Message;
                                }
                                else
                                {
                                    ErrorMess = readAsStringAsync.Result;
                                }
                            }
                        }
                        content.Dispose();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string PostTextGetCoupons<TResult>(string url, ICouponGetCouponFormModel postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess)
        {
            try
            {
                string result = null;
                HttpRequestMessage request;
                request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Post;
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                request.Content = new ObjectContent<ICouponGetCouponFormModel>(postData, formatter);
                using (HttpClient httpClient = new HttpClient())
                {
                    //1. Create headers
                    //2. Send Request
                    if (heads != null)
                    {
                        foreach (var kvp in heads.ToArray())
                        {
                            if (kvp.Key != null && kvp.Value != null)
                            {
                                if (kvp.Key.Equals("Authorization"))
                                {
                                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", kvp.Value.ToString());
                                }
                                else
                                {
                                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(kvp.Value.ToString()));
                                }
                            }
                        }
                    }

                    using (HttpResponseMessage res = httpClient.SendAsync(request).Result)
                    {
                        var readAsStringAsync = res.Content.ReadAsStringAsync();
                        returnCode = res.StatusCode.GetHashCode();

                        if (returnCode >= 200 && returnCode <= 299)
                        {
                            ErrorMess = ""; result = readAsStringAsync.Result;
                        }
                        else
                        {
                            if (readAsStringAsync.Result is HttpMessage)
                            {
                                ErrorMess = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpMessage>(readAsStringAsync.Result).Message;
                            }
                            else
                            {
                                ErrorMess = readAsStringAsync.Result;
                            }
                        }
                    }
                    request.Dispose();
                    return result;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        //  HTTP PUT
        public string PutTextUpdateCoupon<TResult>(string url, ICouponUpdateFormModel postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess)
        {
            try
            {
                string result = null;
                HttpRequestMessage request;
                request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Put;
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                request.Content = new ObjectContent<ICouponUpdateFormModel>(postData, formatter);
                using (HttpClient httpClient = new HttpClient())
                {
                    //content.Headers.Clear();
                    if (heads != null)
                    {
                        foreach (var kvp in heads.ToArray())
                        {
                            if (kvp.Key != null && kvp.Value != null)
                            {
                                if (kvp.Key.Equals("Authorization"))
                                {
                                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", kvp.Value.ToString());
                                }
                                else
                                {
                                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(kvp.Value.ToString()));
                                }

                            }
                        }
                    }

                    using (HttpResponseMessage res = httpClient.SendAsync(request).Result)
                    {
                        var readAsStringAsync = res.Content.ReadAsStringAsync();
                        returnCode = res.StatusCode.GetHashCode();

                        if (returnCode >= 200 && returnCode <= 299)
                        {
                            ErrorMess = ""; result = readAsStringAsync.Result;
                        }
                        else
                        {
                            if (readAsStringAsync.Result is HttpMessage)
                            {
                                ErrorMess = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpMessage>(readAsStringAsync.Result).Message;
                            }
                            else
                            {
                                ErrorMess = readAsStringAsync.Result;
                            }
                        }
                    }
                    request.Dispose();
                    return result;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //  HTTP Insert Receipt Data
        public string PutInsertReceiptData<TResult>(string url, ICouponInserReceiptFormModel postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess)
        {
            try
            {
                string result = null;
                HttpRequestMessage request;
                request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Put;
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                request.Content = new ObjectContent<ICouponInserReceiptFormModel>(postData, formatter);
                using (HttpClient httpClient = new HttpClient())
                {
                    //content.Headers.Clear();
                    if (heads != null)
                    {
                        foreach (var kvp in heads.ToArray())
                        {
                            if (kvp.Key != null && kvp.Value != null)
                            {
                                if (kvp.Key.Equals("Authorization"))
                                {
                                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", kvp.Value.ToString());
                                }
                                else
                                {
                                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(kvp.Value.ToString()));
                                }

                            }
                        }
                    }

                    using (HttpResponseMessage res = httpClient.SendAsync(request).Result)
                    {
                        var readAsStringAsync = res.Content.ReadAsStringAsync();
                        returnCode = res.StatusCode.GetHashCode();

                        if (returnCode >= 200 && returnCode <= 299)
                        {
                            ErrorMess = "";
                            result = readAsStringAsync.Result;
                        }
                        else
                        {
                            if (readAsStringAsync.Result is HttpMessage)
                            {
                                ErrorMess = Newtonsoft.Json.JsonConvert.DeserializeObject<HttpMessage>(readAsStringAsync.Result).Message;
                            }
                            else
                            {
                                ErrorMess = readAsStringAsync.Result;
                            }
                        }
                    }
                    request.Dispose();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
