using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO
{
    public interface IInvoicesDAO
    {
        /// <summary>
        /// Returns id of new invoice that was created during cancelation of selected invoice
        /// </summary>
        /// <param name="db"> DB Connection </param>
        /// <param name="InvoiceId"> Invoice </param>
        /// <param name="PosInfoId"> PosInfo </param>
        /// <param name="StaffId"> Staff </param>
        /// <param name="newInvoiceId"> New Invoice </param>
        /// <returns> New invoice id </returns>
        int cancelReceipt(IDbConnection db, long InvoiceId, long PosInfoId, long StaffId, bool CancelOrder, out long NewInvoiceId);

        long getTicketCount(IDbConnection db, long posInfo);

        /// <summary>
        /// Get Active invoices by OrderNo
        /// </summary>
        /// <param name="db"></param>
        /// <param name="orderno"></param>
        /// <param name="EndOfDayId"></param>
        /// <returns></returns>
        List<InvoicesDTO> InvoicesByOrderNo(IDbConnection db, string orderno);
    }
}
