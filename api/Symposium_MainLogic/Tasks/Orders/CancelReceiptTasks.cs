using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{

    /// <summary>
    /// class that handles receipt cancel
    /// </summary>
    public class CancelReceiptTasks : ICancelReceiptTasks
    {

        IInvoicesDT invoicesDB;
        IOrderDetailInvoicesDT orderDetailInvoicesDB;
        IInvoiceTypesDT invoiceTypesDB;
        IPosInfoDT posInfoDB;
        IPosInfoDetailDT posInfoDetailDB;

        public CancelReceiptTasks(IInvoicesDT invDB, IOrderDetailInvoicesDT detailInvDB, IInvoiceTypesDT invTypesDB, IPosInfoDT posDB, IPosInfoDetailDT posDetDB)
        {
            this.invoicesDB = invDB;
            this.orderDetailInvoicesDB = detailInvDB;
            this.invoiceTypesDB = invTypesDB;
            this.posInfoDB = posDB;
            this.posInfoDetailDB = posDetDB;
        }

        /// <summary>
        /// Checks if selected invoice can be canceled
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice to cancel </param>
        /// <returns> True if receipt can be canceled, false otherwise </returns>
        public bool checkReceiptCancelable(DBInfoModel Store, long InvoiceId)
        {
            InvoiceModel invoice;
            invoice = invoicesDB.GetSingleInvoice(Store, InvoiceId);
            if (invoice == null)
            {
                string err = "Invoice not found for InvoiceId: " + InvoiceId.ToString();
                throw new Exception(err);
            }
            if (invoice.IsPrinted == false || invoice.IsVoided == true || invoice.IsDeleted == true || invoice.IsPaid != 2)
            {
                return false;
            }
            InvoiceTypeModel invoiceType;
            long InvoiceTypeId = invoice.InvoiceTypeId ?? 0;
            invoiceType = invoiceTypesDB.GetSingleInvoiceType(Store, InvoiceTypeId);
            if (invoiceType == null)
            {
                string err = string.Format(Symposium.Resources.Errors.INVOICETYPENOTFOUND, InvoiceTypeId.ToString());
                throw new Exception(err);
            }
            if (invoiceType.Type == 2 || invoiceType.Type == 3 || invoiceType.Type == 8)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Cancels receipt
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice to cancel </param>
        /// <param name="PosInfoId"> Pos </param>
        /// <param name="StaffId"> Staff </param>
        /// <param name="newInvoiceId"> New Invoice </param>
        /// <returns> Id of new invoice inserted </returns>
        public int cancelReceipt(DBInfoModel Store, long InvoiceId, long PosInfoId, long StaffId, bool CancelOrder, out long NewInvoiceId)
        {
            return invoicesDB.cancelReceipt(Store, InvoiceId, PosInfoId, StaffId, CancelOrder, out NewInvoiceId);
        }

        /// <summary>
        /// Gathers data for signalR messages
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceId"> Invoice that was created after cancel </param>
        /// <param name="PosInfoId"> Pos </param>
        /// <returns> SignalRAfterInvoiceModel that contains data for signalR messages. See: See: <seealso cref="Symposium.Models.Models.SignalRAfterInvoiceModel"/> </returns>
        public SignalRAfterInvoiceModel dataToNotifyUsersOnReceiptCancel(DBInfoModel Store, long InvoiceId, long PosInfoId)
        {
            SignalRAfterInvoiceModel dataForSignalR = new SignalRAfterInvoiceModel();

            InvoiceModel invoice;
            invoice = invoicesDB.GetSingleInvoice(Store, InvoiceId);
            InvoiceTypeModel invoiceType;
            long InvoiceTypeId = invoice.InvoiceTypeId ?? 0;
            invoiceType = invoiceTypesDB.GetSingleInvoiceType(Store, InvoiceTypeId);
            PosInfoModel posInfo;
            posInfo = posInfoDB.GetSinglePosInfo(Store, PosInfoId);
            PosInfoDetailModel posInfoDetail;
            long PosInfoDetailId = invoice.PosInfoDetailId ?? 0;
            posInfoDetail = posInfoDetailDB.GetSinglePosInfoDetail(Store, PosInfoDetailId);
            List<OrderDetailInvoicesModel> orderDetailInvoices;
            orderDetailInvoices = orderDetailInvoicesDB.GetOrderDetailInvoicesOfSelectedInvoice(Store, InvoiceId);

            List<long> salesTypes = new List<long>();
            foreach (OrderDetailInvoicesModel item in orderDetailInvoices)
            {
                long salesTypeId = item.SalesTypeId ?? 0;
                if (salesTypeId != 0)
                {
                    bool found = false;
                    foreach (long id in salesTypes)
                    {
                        if (id == salesTypeId)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        salesTypes.Add(salesTypeId);
                    }
                }
            }

            dataForSignalR.InvoiceId = InvoiceId;
            dataForSignalR.SendsVoidToKitchen = posInfoDetail.SendsVoidToKitchen ?? 0;
            dataForSignalR.InvoiceType = invoiceType.Type;
            dataForSignalR.ExtType = null;
            dataForSignalR.ExtecrName = posInfo.FiscalName;
            dataForSignalR.PrintType = PrintTypeEnum.PrintWhole;
            dataForSignalR.ItemAdditionalInfo = null;
            dataForSignalR.TempPrint = false;
            dataForSignalR.PosInfoId = PosInfoId;
            dataForSignalR.TableId = invoice.TableId;
            dataForSignalR.SalesTypes = salesTypes;
            dataForSignalR.kdsMessage = null;
            dataForSignalR.deliveryMessage = null;
            dataForSignalR.useFiscalSignature = true;

            return dataForSignalR;
        }

    }
}
