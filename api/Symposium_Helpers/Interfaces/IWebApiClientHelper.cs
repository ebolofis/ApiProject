using Symposium.Models.Models.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.Helpers.Interfaces
{
    /// <summary>
    /// Rest Client
    /// </summary>
    public interface IWebApiClientHelper
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
        string GetRequest(string url, string user, Dictionary<string, string> headers, out int returnCode, out string ErrorMess, string mediaType = "application/json", string authenticationType = "Basic");

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
        string PostRequest<T>(T model, string url, string user, Dictionary<string, string> headers, out int returnCode, out string ErrorMess, string mediaType = "application/json", string authenticationType = "Basic");

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
        string PatchRequest<T>(T model, string url, string user, Dictionary<string, string> headers, out int returnCode, out string ErrorMess, string mediaType = "application/json", string authenticationType = "Basic");

        string PostFormUrlEncoded<TResult>(string url, IEnumerable<KeyValuePair<string, string>> postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess);

        string PostTextGetCoupons<TResult>(string url, ICouponGetCouponFormModel postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess);

        string PutTextUpdateCoupon<TResult>(string url, ICouponUpdateFormModel postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess);

        string PutInsertReceiptData<TResult>(string url, ICouponInserReceiptFormModel postData, Dictionary<string, string> heads, out int returnCode, out string ErrorMess);
    }
}
