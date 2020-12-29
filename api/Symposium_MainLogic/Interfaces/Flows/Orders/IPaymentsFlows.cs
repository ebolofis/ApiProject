using Symposium.Models.Models;
using Symposium.Models.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.Orders
{
    public interface IPaymentsFlows
    {
        void ApplyPmsCharge(DBInfoModel dbInfo, PMSChargeModel pmsCharge);
        void UpdatePmsHtmlReceipt(DBInfoModel dbInfo, PMSReceiptHTMLModel pmsReceiptHtml);
    }
}
