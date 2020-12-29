using Newtonsoft.Json;
using RestSharp;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Models.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Classes
{
    /// <summary>
    /// Rest Client using RestSharp
    /// </summary>
    public class WebApiClientRestSharpHelper : IWebApiClientRestSharpHelper
    {

        /// <summary>
        /// Returns a result using RestRequest get method
        /// parametes user, headers,mediatype and authenticationType not used
        /// </summary>
        /// <param name="url"></param>
        /// <param name="user"></param>
        /// <param name="headers"></param>
        /// <param name="returnCode"></param>
        /// <param name="ErrorMess"></param>
        /// <param name="mediaType"></param>
        /// <param name="authenticationType"></param>
        /// <returns></returns>
        public string GetRequest(string url, string user, Dictionary<string, string> headers, out int returnCode, out string ErrorMess, string mediaType = "application/json", string authenticationType = "Basic")
        {
            returnCode = 200;
            ErrorMess = "";
            string result = "";
            try
            {
                var client = new RestClient(url);
                
                //if Autentication needed then 
                //client.Authenticator = new HttpBasicAuthenticator("user", "password");

                client.Timeout = -1;

                var request = new RestRequest(Method.GET);

                IRestResponse response = client.Execute(request);
                result = response.Content;

                if (string.IsNullOrWhiteSpace(response.ErrorMessage))
                {
                    returnCode = 200;
                }
                else
                {
                    returnCode = 400;
                    ErrorMess = response.ErrorException.ToString();
                }

            }
            catch (Exception ex)
            {
                ErrorMess = "-- " + ex.ToString();
                returnCode = 500;
            }
            return result;
        }

        public string PatchRequest<T>(T model, string url, string user, Dictionary<string, string> headers, out int returnCode, out string ErrorMess, string mediaType = "application/json", string authenticationType = "Basic")
        {
            throw new NotImplementedException();
        }

        public string PostFormUrlEncoded<TResult>(string url, IEnumerable<KeyValuePair<string, string>> postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess)
        {
            throw new NotImplementedException();
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
            ErrorMess = "";
            string result = "";
            RestClient client = new RestClient(url);
            client.Timeout = -1;
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", mediaType);

            setHeaders(client, request, authenticationType, user, headers);
            request.AddParameter("application/json", JsonConvert.SerializeObject(model), ParameterType.RequestBody);
            IRestResponse response;
           
            try
            {
                response = client.Execute(request);
                result = response.Content;
                if (string.IsNullOrWhiteSpace(response.ErrorMessage))
                {
                    returnCode = 200;
                }         
                else
                {
                    returnCode = 400;
                    ErrorMess = response.ErrorException.ToString();
                }
                   
            }
            catch(Exception ex)
            {
                ErrorMess = "-- "+ ex.ToString();
                returnCode = 500;
            }
           
            return result;

        }

        public string PostTextGetCoupons<TResult>(string url, ICouponGetCouponFormModel postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess)
        {
            throw new NotImplementedException();
        }

        public string PutInsertReceiptData<TResult>(string url, ICouponInserReceiptFormModel postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess)
        {
            throw new NotImplementedException();
        }

        public string PutTextUpdateCoupon<TResult>(string url, ICouponUpdateFormModel postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create Headers for the rest client
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="request">RestRequest</param>
        /// <param name="authenticationType">type of authentication (Basic or OAuth2)</param>
        /// <param name="user">user and password or token for Authentication Header. Format for Basic: "Username:Password", Format for OAuth2: "Bearer  ZTdmZmY1Zjc5MTQ4NDQ5ZTEzMzIyZTOQ"</param>
        /// <param name="headers">custom headers </param>
        private void setHeaders(RestClient client, RestRequest request, string authenticationType, string user, Dictionary<string, string> headers)
        {
            //1. Create Authorization header
            if (!string.IsNullOrEmpty(user))
            {
                switch (authenticationType)
                {
                    case "Basic":
                        request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(user)));
                        break;
                    case "OAuth2":
                        request.AddHeader("Authorization", user);
                        break;
                }
            }

            //2. Create custom headers
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    if (key != null && headers[key] != null)
                        request.AddHeader(key, headers[key]);
                }
            }
        }
    }
}
