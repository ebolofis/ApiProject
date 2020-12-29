//using Pos_WebApi.Models;
using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3
{
    [RoutePrefix("api/v3/Accounts")]
    public class AccountsV3Controller : BasicV3Controller
    {
        IAccountFlows flow;

        public AccountsV3Controller(IAccountFlows flow)
        {
            this.flow = flow;
        }

        /// <summary>
        /// Upsert a list of Accounts from Server to Client Store
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost, Route("UpsertAccount")]
        [Authorize]
        public HttpResponseMessage UpsertAccount(List<AccountSched_Model> Model)
        {
            bool isDeliveryStore = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiBaseConfiguration, "DA_IsClient");
            if (!isDeliveryStore)
                throw new Exception("Store is not client of Delivery Agent");

            UpsertListResultModel res = flow.InformTablesFromDAServer(DBInfo, Model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

    

    }
}
