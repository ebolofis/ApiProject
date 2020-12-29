using Pos_WebApi.Modules;
using Symposium.Models.Models.Orders;
using Symposium.WebApi.Controllers.V3;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pos_WebApi.Controllers.V3.Orders
{
    [RoutePrefix("api/v3/Payments")]
    public class PaymentsV3Controller : BasicV3Controller
    {
        IPaymentsFlows paymentFlows;

        public PaymentsV3Controller(IPaymentsFlows _paymentFlows)
        {
            this.paymentFlows = _paymentFlows;
        }

        [HttpPost, Route("ApplyPmsCharge")]
        [Authorize]
        [ValidateModelState]
        public HttpResponseMessage ApplyPmsCharge(PMSChargeModel pmsCharge)
        {
            paymentFlows.ApplyPmsCharge(DBInfo, pmsCharge);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost, Route("UpdatePmsHtmlReceipt")]
        [Authorize]
        [ValidateModelState]
        public HttpResponseMessage UpdatePmsHtmlReceipt(PMSReceiptHTMLModel pmsReceiptHtml)
        {
            paymentFlows.UpdatePmsHtmlReceipt(DBInfo, pmsReceiptHtml);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
