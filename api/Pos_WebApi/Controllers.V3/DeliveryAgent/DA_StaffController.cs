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
    [RoutePrefix("api/v3/da/Staff")]
    public class DA_StaffController : BasicV3Controller
    {
        IDA_StaffFlows staffFlow;

        public DA_StaffController(IDA_StaffFlows _staffFlow)
        {
            this.staffFlow = _staffFlow;
        }

        /// <summary>
        /// Authenticate Staff 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>StaffId</returns>
        [AllowAnonymous]
        [HttpPost, Route("Login")]
        [ValidateModelState]
        [CheckModelForNull]
        public HttpResponseMessage LoginStaff(DALoginStaffModel loginStaffModel)
        {
            long res = staffFlow.LoginStaff(DBInfo, loginStaffModel);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("Id/{id}")]
        public HttpResponseMessage GetStaffById(long id)
        {
            DA_StaffModel res = staffFlow.GetStaffById(DBInfo, id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}