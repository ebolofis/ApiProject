using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IReceiptDetailsForExtecrDT
    {
        /// <summary>
        /// Get Receipt Details For Extecr
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>
        ExtecrReceiptModel GetReceiptDetailsForExtecr(DBInfoModel Store, Int64 invoiceId, bool groupReceiptItems, bool isForKitchen);
        List<VatPercentageModel> VatPercentage(DBInfoModel Store);
        List<VatComboList> VatList(DBInfoModel Store);
    }
}
