using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IInvoiceShippingDetailsTasks
    {

        /// <summary>
        /// Add's new Record To db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddNewInvoiceShippindDetail(DBInfoModel Store, InvoiceShippingDetailsModel item);

        /// <summary>
        /// Return's a record using InvoiceId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        InvoiceShippingDetailsModel GetInvShippingByInvoiceId(DBInfoModel Store, long InvoiceId);

        /// <summary>
        /// Return's a list of Invoice Shipping Details;
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        List<InvoiceShippingDetailsModel> GetListOfInvoiceShippingByInvoicesId(DBInfoModel Store, List<long> InvoiceId);
    }
}
