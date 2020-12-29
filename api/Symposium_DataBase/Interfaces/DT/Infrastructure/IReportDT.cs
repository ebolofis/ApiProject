using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT {
    public interface IReportDT {
        /// <summary>
        /// Constructs a database connection and parse filters to Reports DAO
        /// Collects data from ReportDAO Grouped By Account ID of Transactions and Invoices and parse them to object Totals and sums of amount
        /// Report Analysis Flat model to By Account Models. Collects Invoices and Invoice details joining its lookup entities to provide a flat model.
        /// </summary>
        /// <param name="Store">Primary DB context Initial Store Value from primary v3 override ApiController</param>
        /// <param name="PosInfoId">Filter invoices with endofdayid null and posinfoid if filter is >0 else it is not applied</param>
        /// <param name="DepartmentId">Filter invoices with endofdayid null and DepartmentId if filter is >0 else it is not applied</param>
        /// <param name="StaffId">Filter invoices with endofdayid null and StaffId if filter is >0 else it is not applied</param>
        /// <returns>List of Objects of Account Info ,sum of amount count of receipts, Invoices List and Totals by Vat</returns>
        List<ReportByAccountTotalsModel> GetPosReportsInvoiceModels(DBInfoModel Store, long PosInfoId, long DepartmentId, long StaffId);
    }
}
