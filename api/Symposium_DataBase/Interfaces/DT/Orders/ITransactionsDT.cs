using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface ITransactionsDT
    {

        /// <summary>
        /// Return's a list of transaction based on invoiceId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        List<TransactionsModel> GetTransactionsByInvoiceId(DBInfoModel Store, long InvoiceId);

        /// <summary>
        /// Return's PMSRoom fo TransferToPms and for cash, CC, etc..
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="AccountId"></param>
        /// <param name="PosInfoId"></param>
        /// <returns></returns>
        PmsRoomModel GetPmsRoomForCashForTransferToPMS(DBInfoModel Store, long AccountId, long PosInfoId);

        /// <summary>
        /// add's new transacion to db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewTransaction(DBInfoModel Store, TransactionsModel item);

        /// <summary>
        /// add's new transacion to db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        long AddNewTransaction(IDbConnection db, TransactionsModel item, IDbTransaction dbTransact, out string Error);
    }
}
