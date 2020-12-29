using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IDelivery_CustomersBillingAddressFlows
    {

        /// <summary>
        /// Return's a Delivery Billing Model using External Key
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="CustomerId"></param>
        /// <param name="ExtKey"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        Delivery_CustomersBillingAddressModel GetModelByExternalKey(DBInfoModel dbInfo, long CustomerId, string ExtKey, ExternalSystemOrderEnum ExtType);
    }
}
