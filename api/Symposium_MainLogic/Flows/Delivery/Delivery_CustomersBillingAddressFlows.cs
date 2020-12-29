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
    public class Delivery_CustomersBillingAddressFlows : IDelivery_CustomersBillingAddressFlows
    {
        IDelivery_CustomersBillingAddressTasks task;

        public Delivery_CustomersBillingAddressFlows(IDelivery_CustomersBillingAddressTasks task)
        {
            this.task = task;
        }

        /// <summary>
        /// Return's a Delivery Billing Nodel using External Key
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CustomerId"></param>
        /// <param name="ExtKey"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public Delivery_CustomersBillingAddressModel GetModelByExternalKey(DBInfoModel dbInfo, long CustomerId, string ExtKey, ExternalSystemOrderEnum ExtType)
        {
            return task.GetModelByExternalKey(dbInfo, CustomerId, ExtKey, ExtType);
        }
    }
}
