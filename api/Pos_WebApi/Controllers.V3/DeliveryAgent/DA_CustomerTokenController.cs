using Pos_WebApi.Modules;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{
    [RoutePrefix("api/v3/da/CustomersTokens")]
    public class DA_CustomerTokenController : BasicV3Controller
    {
        IDA_CustomerTokenFlows customerTokenFlows;

        public DA_CustomerTokenController(IDA_CustomerTokenFlows customerTokenFlows)
        {
            this.customerTokenFlows = customerTokenFlows;
        }

        /// <summary>
        /// Gets customer token by customer id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet, Route("GetToken/{customerId}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage GetCustomerToken(long customerId)
        {
            DA_CustomerTokenModel customerToken = customerTokenFlows.GetCustomerToken(DBInfo, customerId);
            return Request.CreateResponse(HttpStatusCode.OK, customerToken);
        }

        /// <summary>
        /// Upserts customer token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("SetToken")]
        [AllowAnonymous]
        [IsDA]
        [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage SetCustomerToken(DATokenModel model)
        {
            long customerTokenId = customerTokenFlows.SetCustomerToken(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, customerTokenId);
        }

        /// <summary>
        /// Deletes customer token by customer id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet, Route("DeleteToken/{customerId}")]
        [Authorize]
        [IsDA]
        public HttpResponseMessage DeleteCustomerToken(long customerId)
        {
            long customerTokenId = customerTokenFlows.DeleteCustomerToken(DBInfo, customerId);
            return Request.CreateResponse(HttpStatusCode.OK, customerTokenId);
        }

    }
}