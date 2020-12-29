using Symposium.Models.Models;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class ReceiptDetailsForExtecrFlows : IReceiptDetailsForExtecrFlows
    {
        IReceiptDetailsForExtecrTasks ReceiptDetailsForExtecrTasks;
        public ReceiptDetailsForExtecrFlows(IReceiptDetailsForExtecrTasks resTasks)
        {
            this.ReceiptDetailsForExtecrTasks = resTasks;
        }


        /// <summary>
        /// Get Receipt Details For Extecr
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>
        public ExtecrReceiptModel GetReceiptDetailsForExtecr(DBInfoModel dbInfo, Int64 invoiceId, bool groupReceiptItems, bool isForKitchen)
        {
            // get the results
            ExtecrReceiptModel ReceiptModelPreview = ReceiptDetailsForExtecrTasks.GetReceiptDetailsForExtecr(dbInfo, invoiceId, groupReceiptItems, isForKitchen);
            ReceiptModelPreview = ReceiptDetailsForExtecrTasks.CalculateVatPrice(ReceiptModelPreview, dbInfo);
            return ReceiptModelPreview;
        }
    }
}
