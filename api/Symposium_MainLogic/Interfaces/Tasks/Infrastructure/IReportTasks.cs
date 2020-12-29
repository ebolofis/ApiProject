using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks {
    public interface IReportTasks {

        /// <summary>
        /// Task creating ReportPreview model 
        /// Collects from ReportDT Grouped By invoice ID  Invoices and parse a list of them to object Totals on calling Flow
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="PosInfoId">Filter invoices with endofdayid null and posinfoid if filter is >0 else it is not applied</param>
        /// <param name="DepartmentId">Filter invoices with endofdayid null and DepartmentId if filter is >0 else it is not applied</param>
        /// <param name="StaffId">Filter invoices with endofdayid null and StaffId if filter is >0 else it is not applied</param>
        /// <returns>List of group by AccountId invoice objects </returns>
        ReportPreviewForPosModel GetPosReportsInvoiceModels(Models.Models.DBInfoModel Store, long PosInfoId, long DepartmentId, long StaffId);

        /// <summary>
        /// Collections of ReportPreviewForPosModel by department and posid 
        /// first collect pos devices of department refered then apply descriptions corrctions over lookups 
        /// pushes ReportPreviewForPosModel into returnlist and interates same proccess through posmodels collected 
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="PosInfoId">actually unused variable as posinfoid is collected from departmentid</param>
        /// <param name="DepartmentId">Variable used to filter posdevices collection by departmentID and handle of single responce Q</param>
        /// <param name="StaffId">If this filter applied then results will be filtered through all posdevices for current staff</param>
        /// <returns>A list of lists of group by AccountId invoice objects</returns>
        List<ReportPreviewForPosModel> GetPosListReportsInvoiceModels(DBInfoModel Store, long PosInfoId, long DepartmentId, long StaffId);

    }
}
