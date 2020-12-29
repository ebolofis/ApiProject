using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class InvoicesFlows : IInvoicesFlows
    {

        /// <summary>
        /// Main Logic Class that handles the Invoice activities
        /// </summary>
        ICancelReceiptTasks cancelReceipt;
        IInvoiceTasks task;
        IInvoiceShippingDetailsTasks invShippTask;
        ITransactionsTasks transactTask;
        IInvoice_Guest_TransTasks invGuestTransTask;
        ICreditTransactionsTasks credTransactTask;
        ITransferToPmsTasks transfToPmsTask;
        IDA_OrderStatusFlows orderStatusFlows;

        public InvoicesFlows(ICancelReceiptTasks canRec, IInvoiceTasks task, IInvoiceShippingDetailsTasks invShippTask,
            ITransactionsTasks transactTask, IInvoice_Guest_TransTasks invGuestTransTask, ICreditTransactionsTasks credTransactTask,
            ITransferToPmsTasks transfToPmsTask, IDA_OrderStatusFlows orderStatusFlows)
        {
            this.cancelReceipt = canRec;
            this.task = task;
            this.invShippTask = invShippTask;
            this.transactTask = transactTask;
            this.invGuestTransTask = invGuestTransTask;
            this.credTransactTask = credTransactTask;
            this.transfToPmsTask = transfToPmsTask;
            this.orderStatusFlows = orderStatusFlows;
        }

        /// <summary>
        /// create aade qr code image, based on provided url and linked to provided invoiceid
        /// </summary>
        /// <param name="DBInfo">DBInfoModel</param>
        /// <param name="InvoiceId">long</param>
        /// <param name="url">string</param>
        /// <returns>long?</returns>
        public long? CreateInvoiceQR(DBInfoModel DBInfo, long InvoiceId, string url)
        {
            return task.CreateInvoiceQR(DBInfo, InvoiceId, url);
        }

        /// <summary>
        /// Cancels a receipt
        /// </summary>
        /// <param name="dbInfo"> Store </param>
        /// <param name="InvoiceId"> Invoice to cancel </param>
        /// <param name="PosInfoId"> Pos </param>
        /// <param name="StaffId"> Staff </param>
        /// <returns> Data for signalR. See: <seealso cref="Symposium.Models.Models.SignalRAfterInvoiceModel"/> </returns>
        public SignalRAfterInvoiceModel CancelReceipt(DBInfoModel dbInfo, long InvoiceId, long PosInfoId, long StaffId, bool CancelOrder)
        {
            bool pass = cancelReceipt.checkReceiptCancelable(dbInfo, InvoiceId);
            if (pass)
            {
                long newInvoiceId;
                int resultStoredProcedure = cancelReceipt.cancelReceipt(dbInfo, InvoiceId, PosInfoId, StaffId, CancelOrder, out newInvoiceId);
                if (resultStoredProcedure == -1)
                {
                    throw new BusinessException(Symposium.Resources.Errors.INVOICECANCELATIONFAILED, Symposium.Resources.Errors.STOREDPROCEDURENOTEXECUTE);
                }
                orderStatusFlows.InsertOrderStatusFromInvoiceId(dbInfo, newInvoiceId);
                SignalRAfterInvoiceModel dataForSignalR = cancelReceipt.dataToNotifyUsersOnReceiptCancel(dbInfo, newInvoiceId, PosInfoId);
                return dataForSignalR;
            }
            else
            {
                throw new BusinessException(Symposium.Resources.Errors.INVOICECANNOTCANCELED, Symposium.Resources.Errors.STOREDPROCEDUREERRORREASONS);
            }
        }

        /// <summary>
        /// Cancels a receipt
        /// </summary>
        /// <param name="dbInfo"> Store </param>
        /// <param name="InvoiceId"> Invoice to cancel </param>
        /// <param name="PosInfoId"> Pos </param>
        /// <param name="StaffId"> Staff </param>
        /// <returns> Data for signalR. See: <seealso cref="Symposium.Models.Models.SignalRAfterInvoiceModel"/> </returns>
        public SignalRAfterInvoiceModel CancelReceipt(DBInfoModel dbInfo, long InvoiceId, long PosInfoId, long StaffId, bool CancelOrder, out string Error)
        {
            Error = "";
            bool pass = cancelReceipt.checkReceiptCancelable(dbInfo, InvoiceId);
            if (pass)
            {
                long newInvoiceId;
                int resultStoredProcedure = cancelReceipt.cancelReceipt(dbInfo, InvoiceId, PosInfoId, StaffId, CancelOrder, out newInvoiceId);
                if (resultStoredProcedure == -1)
                {
                    Error = Symposium.Resources.Errors.INVOICECANCELATIONFAILED + " " + Symposium.Resources.Errors.STOREDPROCEDURENOTEXECUTE;
                    return null;
                }
                SignalRAfterInvoiceModel dataForSignalR = cancelReceipt.dataToNotifyUsersOnReceiptCancel(dbInfo, newInvoiceId, PosInfoId);
                return dataForSignalR;
            }
            else
            {
                Error = Symposium.Resources.Errors.INVOICECANNOTCANCELED + " " + Symposium.Resources.Errors.STOREDPROCEDUREERRORREASONS;
                return null;
            }
        }

        /// <summary>
        /// Cancel's a ΔΠ.
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="PosInfoId"></param>
        /// <param name="StaffId"></param>
        /// <returns></returns>
        public SignalRAfterInvoiceModel Cancel_DP_Receipt(DBInfoModel dbInfo, long InvoiceId, long PosInfoId, long StaffId, out string Error)
        {
            long newInvoiceId;
            Error = "";
            int resultStoredProcedure = cancelReceipt.cancelReceipt(dbInfo, InvoiceId, PosInfoId, StaffId, true, out newInvoiceId);
            if (resultStoredProcedure == -1)
            {
                Error = Symposium.Resources.Errors.INVOICECANCELATIONFAILED + " " + Symposium.Resources.Errors.STOREDPROCEDURENOTEXECUTE;
                return null;
            }
            SignalRAfterInvoiceModel dataForSignalR = cancelReceipt.dataToNotifyUsersOnReceiptCancel(dbInfo, newInvoiceId, PosInfoId);
            return dataForSignalR;
        }

        /// <summary>
        /// Add's new Invoice to db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewInvoice(DBInfoModel dbInfo, InvoiceModel item)
        {
            return task.AddNewInvoice(dbInfo, item);
        }

        /// <summary>
        /// Return's all invoices using OrderId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<InvoiceModel> GetInvoicesByOrderId(IDbConnection db, long OrderId, IDbTransaction dbTransact = null)
        {
            return task.GetInvoicesByOrderId(db, OrderId, dbTransact);
        }

        /// <summary>
        /// Return's all invoices using OrderId without transaction
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public List<InvoiceModel> GetInvoicesByOrderId(DBInfoModel Store, long OrderId)
        {
            return task.GetInvoicesByOrderId(Store, OrderId);
        }

        /// <summary>
        /// Return's all invoices using List Of Invoice Id
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        public List<InvoiceModel> GetInvoicesByIds(DBInfoModel dbInfo, List<long> InvoicesId)
        {
            return task.GetInvoicesByIds(dbInfo, InvoicesId);
        }

        /// <summary>
        /// Return's a list of InvoiceWithTablesModel for all Invoices associated to OrderId
        /// with all tables associated to each invoice
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="InvoicesId"></param>
        /// <returns></returns>
        public List<InvoiceWithTablesModel> GetOrderInvoices(DBInfoModel dbInfo, List<long> InvoicesId)
        {
            List<InvoiceWithTablesModel> ret = new List<InvoiceWithTablesModel>();
            //Get list of Invoices
            ret = AutoMapper.Mapper.Map<List<InvoiceWithTablesModel>>(task.GetInvoicesByIds(dbInfo, InvoicesId));

            //Gt List of Invoice Shipping Detail for all InvoicesIds
            List<InvoiceShippingDetailsModel> invoiceShipping = invShippTask.GetListOfInvoiceShippingByInvoicesId(dbInfo, InvoicesId);

            //List Of Transaction for Specific orderId and InvoicesId
            List<TransactionsExtraModel> transactions = transactTask.GetTransactionsByInvoiceId(dbInfo, InvoicesId);

            List<long> transactIds = transactions.Select(s => s.Id).Distinct().ToList();

            //List Of Invoice_Guests_TransModel
            List<Invoice_Guests_TransModel> invGuest = invGuestTransTask.GetInvoiceGuestByInvoiceIdAndTransactId(dbInfo, InvoicesId, transactIds);

            //List of Credit Transactions
            List<CreditTransactionsModel> creditTransactions = credTransactTask.GetCreditTransactionsByInvoiceIdAndTransactionId(dbInfo, InvoicesId, transactIds);

            //List Of Transfer To Pms
            List<TransferToPmsModel> transfToPms = transfToPmsTask.GetModelByTransactionId(dbInfo, transactIds);

            //Fill TransactionsExtraModel with associated tables 
            foreach (TransactionsExtraModel item in transactions)
            {
                item.InvoiceGuest = invGuest.Where(w => w.TransactionId == item.Id).Select(s => s).ToList();
                item.CreditTransaction = creditTransactions.Where(w => w.TransactionId == item.Id).Select(s => s).ToList();
                item.TransferToPms = transfToPms.Where(w => w.TransactionId == item.Id).Select(s => s).ToList();
            }

            //Fill InvoiceWithTablesModel with associated tables
            foreach (InvoiceWithTablesModel item in ret)
            {
                item.Transactions = transactions.Where(w => w.InvoicesId == item.Id).Select(s => s).ToList();
                item.InvoiceShippings = invoiceShipping.Where(w => w.InvoicesId == item.Id).Select(s => s).ToList();
            }

            return ret;
        }

        /// <summary>
        /// Update's the field IsPrinted for table Invoices and OrderDetailInvoices for Specific invoie Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="IsPrinted"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        public bool UpdateInvoicePrinted(DBInfoModel Store, long InvoiceId, bool IsPrinted, out string Error)
        {
            return task.UpdateInvoicePrinted(Store, InvoiceId, IsPrinted, out Error);
        }

        /// <summary>
        /// Get Invoice from Old Invoice Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public InvoiceWithTablesModel GetInvoiceFromOldId(DBInfoModel Store, long InvoiceId)
        {
            return task.GetInvoiceFromOldId(Store, InvoiceId);
        }

        /// <summary>
        /// Get Invoice from Old Invoice Id using transaction
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbTransaction"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public InvoiceWithTablesModel GetInvoiceFromOldId(IDbConnection db, IDbTransaction dbTransaction, long InvoiceId)
        {
            return task.GetInvoiceFromOldId(db, dbTransaction, InvoiceId);
        }
    }
}
