using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IInvoiceShippingDetailsDT
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
        /// Add's new Record To db
        /// </summary>
        /// <param name="db"></param>
        /// <param name="item"></param>
        /// <param name="dbTransact"></param>
        /// <param name="Error"></param>
        /// <returns></returns>
        long AddNewInvoiceShippindDetail(IDbConnection db, InvoiceShippingDetailsModel item, IDbTransaction dbTransact, out string Error);
    }
}
