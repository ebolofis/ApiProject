using Symposium.Models.Models;
using Symposium.Models.Models.Orders;
using Symposium.WebApi.DataAccess.Interfaces.DT.Orders;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.Orders
{
    public class PaymentsTasks : IPaymentsTasks
    {
        IPaymentsDT paymentsDT;

        public PaymentsTasks(IPaymentsDT _paymentsDT)
        {
            this.paymentsDT = _paymentsDT;
        }

        public List<TransferToPmsModel> CreateTransferModel(PMSCustomerModel customer, List<PMSDepartmentModel> departments, int hotelId)
        {
            List<TransferToPmsModel> pmsTransferList = new List<TransferToPmsModel>();
            foreach (PMSDepartmentModel department in departments)
            {
                TransferToPmsModel pmsTransferModel = new TransferToPmsModel();
                pmsTransferModel.RegNo = customer.ReservationId.ToString();
                pmsTransferModel.ProfileId = customer.ProfileNo.ToString();
                pmsTransferModel.ProfileName = customer.FirstName + " " + customer.LastName;
                pmsTransferModel.RoomId = customer.RoomId.ToString();
                pmsTransferModel.RoomDescription = customer.Room;
                pmsTransferModel.PmsDepartmentId = department.Id.ToString();
                pmsTransferModel.PmsDepartmentDescription = department.Description;
                pmsTransferModel.Total = department.Amount;
                pmsTransferModel.HotelId = hotelId;
                pmsTransferModel.Description = "ApplyPmsCharge";
                pmsTransferModel.TransferType = 0;
                pmsTransferModel.Status = 0;
                pmsTransferModel.SendToPMS = true;
                pmsTransferModel.SendToPmsTS = DateTime.Now;
                pmsTransferModel.TransferIdentifier = Guid.NewGuid().ToString();
                pmsTransferList.Add(pmsTransferModel);
            }
            return pmsTransferList;
        }

        public bool InsertPmsCharges(DBInfoModel dbInfo, List<TransferToPmsModel> pmsTransfers)
        {
            bool chargesInserted = paymentsDT.InsertPmsCharges(dbInfo, pmsTransfers);
            return chargesInserted;
        }

        public List<long> GetTransactionIds(List<TransactionsModel> transactions)
        {
            List<long> transactionIds = new List<long>();
            foreach (TransactionsModel transaction in transactions)
            {
                transactionIds.Add(transaction.Id);
            }
            return transactionIds;
        }

        public void UpdatePmsWithHtmlReceipt(List<TransferToPmsModel> transfersToPMS, PMSReceiptHTMLModel pmsReceiptHtml)
        {
            foreach (TransferToPmsModel transferToPms in transfersToPMS)
            {
                transferToPms.InvoiceId = pmsReceiptHtml.InvoiceId;
                transferToPms.HtmlReceipt = Encoding.UTF8.GetBytes(pmsReceiptHtml.HtmlReceipt);
            }
            return;
        }
    }
}
