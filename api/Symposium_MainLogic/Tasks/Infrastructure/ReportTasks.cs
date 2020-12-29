using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System.Collections.Generic;

namespace Symposium.WebApi.MainLogic.Tasks {
    public class ReportTasks: IReportTasks {
        IReportDT reportDT;  // part of DataAccess Layer
        IPosInfoDT posDT;

        public ReportTasks(IReportDT reportDT, IPosInfoDT posDT) {
            this.reportDT = reportDT;
            this.posDT = posDT;
        }
        /// <summary>
        /// Task creating ReportPreview model 
        /// Collects from ReportDT Grouped By invoice ID  Invoices and parse a list of them to object Totals on calling Flow
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="PosInfoId">Filter invoices with endofdayid null and posinfoid if filter is >0 else it is not applied</param>
        /// <param name="DepartmentId">Filter invoices with endofdayid null and DepartmentId if filter is >0 else it is not applied</param>
        /// <param name="StaffId">Filter invoices with endofdayid null and StaffId if filter is >0 else it is not applied</param>
        /// <returns>List of group by AccountId invoice objects </returns>
        public ReportPreviewForPosModel GetPosReportsInvoiceModels(DBInfoModel Store, long PosInfoId, long DepartmentId, long StaffId) {
            ReportPreviewForPosModel previewModel = new ReportPreviewForPosModel();
            previewModel.PosInfoId = PosInfoId;
            previewModel.StaffId = StaffId;
            previewModel.DepartmentId = DepartmentId;
            previewModel.PosInfoDescription = "";
            List<ReportByAccountTotalsModel> flat = reportDT.GetPosReportsInvoiceModels(Store, PosInfoId, DepartmentId, StaffId);
            previewModel.Totals = flat;

            return previewModel;
        }
        /// <summary>
        /// Collections of ReportPreviewForPosModel by department and posid 
        /// first collect pos devices of department refered then apply descriptions corrctions over lookups 
        /// pushes ReportPreviewForPosModel into returnlist and interates same proccess through posmodels collected 
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="PosInfoId">actually unused variable as posinfoid is collected from departmentid</param>
        /// <param name="DepartmentId">Variable used to filter posdevices collection by departmentID and handle of single responce Q</param>
        /// <param name="StaffId">If this filter applied (>0) then results will be filtered through all posdevices for current staff</param>
        /// <returns>A list of lists of group by AccountId invoice objects</returns>
        public List<ReportPreviewForPosModel> GetPosListReportsInvoiceModels(DBInfoModel Store, long PosInfoId, long DepartmentId, long StaffId) {
            List<ReportPreviewForPosModel> retlist = new List<ReportPreviewForPosModel>();
            List<PosInfoModel> poses = posDT.GetDepartmentPosInfo(Store, DepartmentId);
            foreach (PosInfoModel pos in poses) {
                ReportPreviewForPosModel previewModel = GetPosReportsInvoiceModels(Store, pos.Id, pos.DepartmentId ?? 0, StaffId);
                previewModel.PosInfoDescription = pos.Description;
                retlist.Add(previewModel);
            }

            return retlist;
        }
    }
}
