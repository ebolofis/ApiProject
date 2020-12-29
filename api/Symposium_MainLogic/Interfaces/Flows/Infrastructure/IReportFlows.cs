using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows {
    /// <summary>
    /// handles the Report activities
    /// </summary>
    public interface IReportFlows {
        /// <summary>
        /// Flow creating ReportPrevie model 
        /// Collects from ReportTasks Grouped By invoice ID  Invoices and parse them to object Totals
        /// Calls Generic Task to constuct Lookups Descriptions 
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="PosInfoId">Filter invoices with endofdayid null and posinfoid if filter is >0 else it is not applied</param>
        /// <param name="DepartmentId">Filter invoices with endofdayid null and DepartmentId if filter is >0 else it is not applied</param>
        /// <param name="StaffId">Filter invoices with endofdayid null and StaffId if filter is >0 else it is not applied</param>
        /// <returns>List of objects with pos staff and department and a totals list of grouped by accountId invoices in a list with total amount</returns>
        ReportPreviewForPosModel GetReportTotalsForPos(DBInfoModel Store, long PosInfoId, long DepartmentId, long StaffId);


        List<ReportPreviewForPosModel> GetReportTotalsForDepartment(DBInfoModel Store, long PosInfoId, long DepartmentId, long StaffId);

        }
}
