using Symposium.Models.Enums;
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
    public class Delivery_CustomersShippingAddressFlows : IDelivery_CustomersShippingAddressFlows
    {
        IDelivery_CustomersShippingAddressTasks task;

        public Delivery_CustomersShippingAddressFlows(IDelivery_CustomersShippingAddressTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// Return's A Model bases on an External Key and External Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CustomerId"></param>
        /// <param name="ExternalKey"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public Delivery_CustomersShippingAddressModel GetModelByExternalKey(DBInfoModel dbInfo, long CustomerId, string ExternalKey, ExternalSystemOrderEnum ExtType)
        {
            return task.GetModelByExternalKey(dbInfo, CustomerId, ExternalKey, ExtType);
        }
    }
}
