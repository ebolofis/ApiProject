using Pos_WebApi.Modules;
using Symposium.Models.Enums;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.DeliveryAgent
{


    [RoutePrefix("api/v3/da/Efood")]
    public class DA_EfoodController : BasicV3Controller
    {
        IDA_EfoodFlows efoodFlow;

        public DA_EfoodController(IDA_EfoodFlows _efoodFlow)
        {
            this.efoodFlow = _efoodFlow;
        }

        /// <summary>
        /// Get orders from efood. 
        /// </summary>
        /// <param name="Model">DA_EfoodModel</param>
        /// <returns></returns>
        [HttpPost, Route("PostEfoodOrders")]
        public HttpResponseMessage PostEfoodNewOrders(DA_EfoodModel Model)
        {
            efoodFlow.PostEfoodNewOrders(Model);
            return Request.CreateResponse(HttpStatusCode.OK, "Order Has Send");
        }
    }
}