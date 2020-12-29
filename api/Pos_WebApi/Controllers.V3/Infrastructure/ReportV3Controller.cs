using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Symposium.WebApi.Controllers.V3 {
    [RoutePrefix("api/v3/Report")]
    public class ReportV3Controller : BasicV3Controller {
        /// <summary>
        /// Main Logic Interface for reports Flow 
        /// </summary>
        IReportFlows report;

        public ReportV3Controller(IReportFlows rep) {
            this.report = rep;
        }
        /// <summary>
        /// Resource to provide Grouped Stats by AccountId of invoices from transactions that has no endofDay
        /// Calls Report FLow to provide results from reports tasks and general object fields by generic tasks 
        /// </summary>
        /// <param name="Store">Object Created by primary v3 controller of resources to define connections</param>
        /// <param name="PosInfoId">Filter invoices with endofdayid null and posinfoid if filter is >0 else it is not applied</param>
        /// <param name="DepartmentId">Filter invoices with endofdayid null and DepartmentId if filter is >0 else it is not applied</param>
        /// <param name="StaffId">Filter invoices with endofdayid null and StaffId if filter is >0 else it is not applied</param>
        /// <returns>Objects with pos staff and department references and a totals array as invoices list of grouped by accountId invoices in a list with total amount</returns>
        [HttpGet, Route("ReportTotalsForPos/{PosInfoId}/{DepartmentId}/{StaffId}")]
        public HttpResponseMessage ReportTotalsForPos(long PosInfoId, long DepartmentId, long StaffId) {
            try {
                ReportPreviewForPosModel res = report.GetReportTotalsForPos(DBInfo, PosInfoId, DepartmentId, StaffId);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }

        }

        /// <summary>
        /// Resource to provide Grouped Stats by AccountId of invoices from transactions that has no endofDay
        /// Calls Report FLow to provide results from reports tasks and general object fields by generic tasks 
        /// Collects posinfoIds from department lookup and provide a list of ReportPreviewForPosModel by department and posinfo
        /// </summary>
        /// <param name="Store">Object Created by primary v3 controller of resources to define connections</param>
        /// <param name="PosInfoId">Filter invoices with endofdayid null and posinfoid if filter is >0 else it is not applied</param>
        /// <param name="DepartmentId">Filter invoices with endofdayid null and DepartmentId if filter is >0 else it is not applied</param>
        /// <param name="StaffId">Filter invoices with endofdayid null and StaffId if filter is >0 else it is not applied</param>
        /// <returns>List of objects with pos staff and department and a totals array as invoices list of grouped by accountId invoices in a list with total amount</returns>
        [HttpGet, Route("ReportTotalsForDepartment/{PosInfoId}/{DepartmentId}/{StaffId}")]
        public HttpResponseMessage ReportTotalsForDepartment(long PosInfoId, long DepartmentId, long StaffId) {
            try {
                List<ReportPreviewForPosModel> res = report.GetReportTotalsForDepartment(DBInfo, PosInfoId, DepartmentId, StaffId);
                return Request.CreateResponse(HttpStatusCode.OK, res);
            } catch (Exception ex) {
                logger.Error(ex.ToString());
                return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message);
            }

        }
    }
}
