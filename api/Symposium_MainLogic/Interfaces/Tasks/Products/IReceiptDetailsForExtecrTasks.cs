using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IReceiptDetailsForExtecrTasks
    {
        /// <summary>
        /// Get Receipt Details For Extecr
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>
        ExtecrReceiptModel GetReceiptDetailsForExtecr(DBInfoModel Store, Int64 invoiceId, bool groupReceiptItems, bool isForKitchen);

        /// <summary>
        /// Calculate Vat Of Price
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        ExtecrReceiptModel CalculateVatPrice(ExtecrReceiptModel model, DBInfoModel Store);
    }
}
