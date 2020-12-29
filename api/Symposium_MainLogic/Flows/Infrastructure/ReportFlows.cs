using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using System;
using System.Collections.Generic;

namespace Symposium.WebApi.MainLogic.Flows {
    /// <summary>
    /// Main Logic Class that handles the Report activities
    /// </summary>
    public class ReportFlows : IReportFlows {
        IReportTasks rtask;
        public ReportFlows(IReportTasks rtask) {
            this.rtask = rtask;
        }
        /// <summary>
        /// Flow creating ReportPreview model 
        /// Collects from ReportTasks Grouped By invoice ID  Invoices and parse them to object Totals
        /// Calls Generic Task to constuct Lookups Descriptions 
        /// </summary>
        /// <param name="dbInfo">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="PosInfoId">Filter invoices with endofdayid null and posinfoid if filter is >0 else it is not applied</param>
        /// <param name="DepartmentId">Filter invoices with endofdayid null and DepartmentId if filter is >0 else it is not applied</param>
        /// <param name="StaffId">Filter invoices with endofdayid null and StaffId if filter is >0 else it is not applied</param>
        /// <returns>List of objects with pos staff and department and a totals list of grouped by accountId invoices in a list with total amount</returns>
        public ReportPreviewForPosModel GetReportTotalsForPos(DBInfoModel dbInfo, long PosInfoId, long DepartmentId, long StaffId) {
            return rtask.GetPosReportsInvoiceModels(dbInfo, PosInfoId, DepartmentId, StaffId);
        }

        /// <summary>
        /// Flow creating ReportPreview model 
        /// Collects from ReportTasks Grouped By invoice ID  Invoices and parse them to object Totals
        /// Calls Generic Task to constuct Lookups Descriptions 
        /// </summary>
        /// <param name="dbInfo">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="PosInfoId">Filter invoices with endofdayid null and posinfoid if filter is >0 else it is not applied</param>
        /// <param name="DepartmentId">Filter invoices with endofdayid null and DepartmentId if filter is >0 else it is not applied</param>
        /// <param name="StaffId">Filter invoices with endofdayid null and StaffId if filter is >0 else it is not applied</param>
        /// <returns>List of objects with pos staff and department and a totals list of grouped by accountId invoices in a list with total amount</returns>
        public List<ReportPreviewForPosModel> GetReportTotalsForDepartment(DBInfoModel dbInfo, long PosInfoId, long DepartmentId, long StaffId) {
            return rtask.GetPosListReportsInvoiceModels(dbInfo, PosInfoId, DepartmentId, StaffId);
        }


    }
}
