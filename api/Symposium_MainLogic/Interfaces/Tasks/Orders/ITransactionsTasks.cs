using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface ITransactionsTasks
    {

        /// <summary>
        /// Create's List Of Transactions with all connected tables for each list items. Tables are
        /// Invoice Guest Transaction
        /// Credits
        /// Transfer To Pms
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="receipt"></param>
        /// <param name="inv"></param>
        /// <param name="HotelInfoId"></param>
        /// <returns></returns>
        List<TransactionsExtraModel> ReturnTransactionFromReceipt(DBInfoModel Store, Receipts receipt, InvoiceModel inv, int HotelInfoId = -1);

        /// <summary>
        /// Return's a list of transaction based on invoiceId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        List<TransactionsModel> GetTransactionsByInvoiceId(DBInfoModel Store, long InvoiceId);

        /// <summary>
        /// add's new transacion to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewTransaction(DBInfoModel Store, TransactionsModel item);

        /// <summary>
        /// Return a list of Transaction Extra Model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        List<TransactionsExtraModel> GetTransactionsByInvoiceId(DBInfoModel Store, List<long> InvoiceId);

        /// <summary>
        /// Return's PMS Room For TransferToPms for Cash, CC, etc....
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="AccountId"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        PmsRoomModel GetPmsRoomForCashForTransferToPMS(DBInfoModel Store, long AccountId, long PosInfoId);
    }
}
