using Symposium.Models.Models;
using Symposium.Models.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.Orders
{
    public interface IPaymentsTasks
    {
        List<TransferToPmsModel> CreateTransferModel(PMSCustomerModel customer, List<PMSDepartmentModel> departments, int hotelId);

        bool InsertPmsCharges(DBInfoModel dbInfo, List<TransferToPmsModel> pmsTransfers);

        List<long> GetTransactionIds(List<TransactionsModel> transactions);

        void UpdatePmsWithHtmlReceipt(List<TransferToPmsModel> transfersToPMS, PMSReceiptHTMLModel pmsReceiptHtml);
    }
}
