using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IInvoiceTypeTasks
    {

        /// <summary>
        /// Get's First InvoiceType Record using Field Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        InvoiceTypeModel GetInvoiceTypeByType(DBInfoModel Store, Int16 Type);

        /// <summary>
        /// Returns invoice type with selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceTypeId"> Invoice Type </param>
        /// <returns> Invoice type  model. See: <seealso cref="Symposium.Models.Models.InvoiceTypeModel"/> </returns>
        InvoiceTypeModel GetSingleInvoiceType(DBInfoModel Store, long InvoiceTypeId);
    }
}
