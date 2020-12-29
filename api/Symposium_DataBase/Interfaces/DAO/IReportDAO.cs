using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO {
    public interface IReportDAO {
        /// <summary>
        /// Get general filters by caller with a database connection constucts a dapper string query
        /// Then on dupper split handle result function groups list of invoices and vats , constuct totals for groups then returns data to primary Task.
        /// Applied function to results produce from flat model
        /// Totals by VatId foreach Invoice of its invoicedetails and Totals by VatId of invoices by AccountId from InvoiceDetails Price by % payment Total of transaction by accountid Total
        /// This % total purpose is dynamically created for each invoice payment cause there is an inipendent by items % of cost and payment
        /// </summary>
        /// <param name="PosInfoId">Filter invoices with endofdayid null and posinfoid if filter is >0 else it is not applied</param>
        /// <param name="DepartmentId">Filter invoices with endofdayid null and DepartmentId if filter is >0 else it is not applied</param>
        /// <param name="StaffId">Filter invoices with endofdayid null and StaffId if filter is >0 else it is not applied</param>
        /// <param name="db">Database instance of DT connection string provided to </param>
        /// <returns>List of Receipt Analysis by AccountId Total Models</returns>
        List<ReportByAccountTotalsModel> OpenEndOfDayStats(long PosInfoId, long DepartmentId, long StaffId, IDbConnection db);
    }
}
