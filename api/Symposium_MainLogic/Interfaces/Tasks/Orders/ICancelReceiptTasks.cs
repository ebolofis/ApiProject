using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{

    /// <summary>
    /// class that handles receipt cancel
    /// </summary>
    public interface ICancelReceiptTasks
    {

        /// <summary>
        /// Checks if selected invoice can be canceled
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice to cancel </param>
        /// <returns> True if receipt can be canceled, false otherwise </returns>
        bool checkReceiptCancelable(DBInfoModel Store, long InvoiceId);

        /// <summary>
        /// Cancels receipt
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice to cancel </param>
        /// <param name="PosInfoId"> Pos </param>
        /// <param name="StaffId"> Staff </param>
        /// <param name="newInvoiceId"> New Invoice </param>
        /// <returns> Id of new invoice inserted </returns>
        int cancelReceipt(DBInfoModel Store, long InvoiceId, long PosInfoId, long StaffId, bool CancelOrder, out long NewInvoiceId);

        /// <summary>
        /// Gathers data for signalR messages
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice that was created after cancel </param>
        /// <param name="PosInfoId"> Pos </param>
        /// <returns> SignalRAfterInvoiceModel that contains data for signalR messages. See: See: <seealso cref="Symposium.Models.Models.SignalRAfterInvoiceModel"/> </returns>
        SignalRAfterInvoiceModel dataToNotifyUsersOnReceiptCancel(DBInfoModel Store, long InvoiceId, long PosInfoId);
    }
}
