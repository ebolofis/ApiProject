using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IInvoiceShippingDetailsFlows
    {

        /// <summary>
        /// Add's new Record To db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewInvoiceShippindDetail(DBInfoModel Store, InvoiceShippingDetailsModel item);
    }
}
