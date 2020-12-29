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
    public class InvoiceTypeTasks : IInvoiceTypeTasks
    {
        IInvoiceTypesDT dt;

        public InvoiceTypeTasks(IInvoiceTypesDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Get's First InvoiceType Record using Field Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public InvoiceTypeModel GetInvoiceTypeByType(DBInfoModel Store, Int16 Type)
        {
            return dt.GetInvoiceTypeByType(Store, Type);
        }

        /// <summary>
        /// Returns invoice type with selected Id
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="InvoiceTypeId"> Invoice Type </param>
        /// <returns> Invoice type  model. See: <seealso cref="Symposium.Models.Models.InvoiceTypeModel"/> </returns>
        public InvoiceTypeModel GetSingleInvoiceType(DBInfoModel Store, long InvoiceTypeId)
        {
            return dt.GetSingleInvoiceType(Store, InvoiceTypeId);
        }
    }
}
