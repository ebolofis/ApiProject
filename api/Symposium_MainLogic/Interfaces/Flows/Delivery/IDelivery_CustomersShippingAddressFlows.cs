using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IDelivery_CustomersShippingAddressFlows 
    {

        /// <summary>
        /// Return's A Model bases on an External Key and External Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CustomerId"></param>
        /// <param name="ExternalKey"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        Delivery_CustomersShippingAddressModel GetModelByExternalKey(DBInfoModel Store, long CustomerId, string ExternalKey, ExternalSystemOrderEnum ExtType);
    }
}
