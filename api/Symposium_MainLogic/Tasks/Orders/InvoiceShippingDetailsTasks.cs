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
    public class InvoiceShippingDetailsTasks : IInvoiceShippingDetailsTasks
    {
        IInvoiceShippingDetailsDT dt;

        public InvoiceShippingDetailsTasks(IInvoiceShippingDetailsDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Add's new Record To db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long AddNewInvoiceShippindDetail(DBInfoModel Store, InvoiceShippingDetailsModel item)
        {
            return dt.AddNewInvoiceShippindDetail(Store, item);
        }

        /// <summary>
        /// Return's a record using InvoiceId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public InvoiceShippingDetailsModel GetInvShippingByInvoiceId(DBInfoModel Store, long InvoiceId)
        {
            return dt.GetInvShippingByInvoiceId(Store, InvoiceId);
        }

        /// <summary>
        /// Return's a list of Invoice Shipping Details;
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="InvoiceId"></param>
        /// <returns></returns>
        public List<InvoiceShippingDetailsModel> GetListOfInvoiceShippingByInvoicesId(DBInfoModel Store, List<long> InvoiceId)
        {
            List<InvoiceShippingDetailsModel> ret = new List<InvoiceShippingDetailsModel>();
            foreach (long item in InvoiceId)
                ret.Add(dt.GetInvShippingByInvoiceId(Store, item));
            return ret;
        }
    }
}
