using log4net;
using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.Orders;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Hotel;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Orders;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.Orders
{
    public class PaymentsFlows : IPaymentsFlows
    {
        IHotelFlows hotelFlows;
        IPaymentsTasks paymentsTasks;
        ITransactionsTasks transactionsTasks;
        ITransferToPmsTasks transferToPmsTasks;
        IHotelInfoTasks hotelInfoTasks;
        protected ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PaymentsFlows(IHotelFlows _hotelFlows, IPaymentsTasks _paymentsTasks, ITransactionsTasks _transactionsTasks, ITransferToPmsTasks _transferToPmsTasks, IHotelInfoTasks _hotelInfoTasks)
        {
            this.hotelFlows = _hotelFlows;
            this.paymentsTasks = _paymentsTasks;
            this.transactionsTasks = _transactionsTasks;
            this.transferToPmsTasks = _transferToPmsTasks;
            this.hotelInfoTasks = _hotelInfoTasks;
        }

        public void ApplyPmsCharge(DBInfoModel dbInfo, PMSChargeModel pmsCharge)
        {
            try
            {
                HotelsInfoModel hotelInfo = hotelInfoTasks.SelectHotelInfoById(dbInfo, pmsCharge.HotelInfoId);
                if (hotelInfo != null)
                {
                    bool isCheckedOut = hotelFlows.IsCustomerCheckOut(dbInfo, hotelInfo, pmsCharge.Customer);
                    if (!isCheckedOut)
                    {
                        List<TransferToPmsModel> pmsTransferList = paymentsTasks.CreateTransferModel(pmsCharge.Customer, pmsCharge.Departments, hotelInfo.HotelId ?? 0);
                        bool chargesApplied = paymentsTasks.InsertPmsCharges(dbInfo, pmsTransferList);
                        if (!chargesApplied)
                        {
                            throw new BusinessException("Couldn't insert charges for PMS customer!");
                        }
                    }
                    else
                    {
                        throw new BusinessException("Customer has checked out!");
                    }
                }
                else
                {
                    throw new BusinessException("Couldn't find hotel for customer charge!");
                }
            }
            catch (BusinessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                logger.Error("Error creating charges for PMS customer: " + ex.ToString());
                throw new BusinessException("Couldn't create charges for PMS customer!");
            }
            return;
        }

        public void UpdatePmsHtmlReceipt(DBInfoModel dbInfo, PMSReceiptHTMLModel pmsReceiptHtml)
        {
            try
            {
                List<TransactionsModel> transactions = transactionsTasks.GetTransactionsByInvoiceId(dbInfo, pmsReceiptHtml.InvoiceId);
                if (transactions != null)
                {
                    List<long> transactionIds = paymentsTasks.GetTransactionIds(transactions);
                    List<TransferToPmsModel> transfersToPMS = transferToPmsTasks.GetTransfersToPmsByTransactionIds(dbInfo, transactionIds);
                    if (transfersToPMS != null)
                    {
                        paymentsTasks.UpdatePmsWithHtmlReceipt(transfersToPMS, pmsReceiptHtml);
                        transferToPmsTasks.UpdateTransfersToPms(dbInfo, transfersToPMS);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error updating transfer to PMS with HTML receipt: " + ex.ToString());
            }
            return;
        }

    }
}
