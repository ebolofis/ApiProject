using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/NFCcard")]
    public class NFCcardV3Controller : BasicV3Controller
    {
        /// <summary>
        /// Main Logic Class
        /// </summary>
        INFCcardFlows nfcFlows;

        public NFCcardV3Controller(INFCcardFlows nfc)
        {
            this.nfcFlows = nfc;
        }
        
        /// <summary>
        /// Gets the first and only row of table NFC card
        /// </summary>
        /// <returns>The selected NFC card if all went ok, else 404</returns>
        [HttpGet, Route("GetNFCcardInfo")]
        [AllowAnonymous]
        public HttpResponseMessage NFCcardPreview()
        {
            try
            {
                NFCcardModel results = nfcFlows.SelectNFCcardTable(DBInfo);
                return Request.CreateResponse(HttpStatusCode.OK, results); // return results
            }
            catch (BusinessException bex)
            {
                logger.Warn("Message: " + bex.Message + ", Description: " + bex.Description);
                return Request.CreateResponse(HttpStatusCode.NotFound, bex.Message); //return 404
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }

        /// <summary>
        /// Update the selected NFC row 
        /// </summary>
        /// <param name="nfc"> NFC Row to alter</param>
        /// <returns>200 if created successfully, else 404</returns>
        [HttpPost, Route("UpdateNFCcardInfo")]

        public HttpResponseMessage NFCcardUpdate(NFCcardModel nfc)
        {
            try
            {
                bool success = nfcFlows.UpdateNFCcardTable(DBInfo, nfc);
                return Request.CreateResponse(HttpStatusCode.OK, Symposium.Resources.Messages.NFCCARDUPDATED); // return results
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message); //return 404
            }
        }
    }
}
