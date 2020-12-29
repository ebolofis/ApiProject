using Newtonsoft.Json;
using Symposium.Models.Models;
using Symposium.Models.Models.DateRange;
using Symposium.Models.Models.Payroll;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.Payroll
{
    [RoutePrefix("api/v3/Payroll")]
    public class PayrollController : BasicV3Controller 
    {

        IPayrollFlows payrole;

        public PayrollController(IPayrollFlows payflows)
        {
            this.payrole = payflows;
        }


        [HttpGet, Route("Get")]
        public HttpResponseMessage GetAll()
        {
            List<PayrollNewModel> res = payrole.GetAll(DBInfo);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet, Route("GetTopRowByType/StaffId/{StaffId}")]
        public HttpResponseMessage GetTopRowByType(long StaffId)
        {
            PayrollNewModel res = payrole.GetTopRowByType(DBInfo, StaffId);

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("InsertPayroll")]
        public HttpResponseMessage InsertPayroll(PayrollNewModel model)
        {
            List<PayrollNewModel> res = payrole.InsertPayroll(DBInfo,model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }


        [HttpPost, Route("Update")]

        public HttpResponseMessage Update(PayrollNewModel model)
        {
            List<PayrollNewModel> res = payrole.Update(DBInfo, model);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpPost, Route("Delete/Ιd/{Id}")]
        public HttpResponseMessage DeletePayroll(long Id)
        {
            List<PayrollNewModel> res = payrole.DeletePayroll(DBInfo, Id);
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

    }
}