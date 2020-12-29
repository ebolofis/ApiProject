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
    public class InvoiceShippingDetailsFlows : IInvoiceShippingDetailsFlows
    {
        IInvoiceShippingDetailsTasks task;

        public InvoiceShippingDetailsFlows(IInvoiceShippingDetailsTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// Add's new Record To db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewInvoiceShippindDetail(DBInfoModel dbInfo, InvoiceShippingDetailsModel item)
        {
            return task.AddNewInvoiceShippindDetail(dbInfo, item);
        }

    }
}
