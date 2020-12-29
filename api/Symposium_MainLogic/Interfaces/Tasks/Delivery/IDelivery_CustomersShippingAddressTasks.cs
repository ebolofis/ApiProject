using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IDelivery_CustomersShippingAddressTasks
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

        DeliveryCustomersShippingAddressModel GetAddressByLatLng(DBInfoModel Store, string latitude, string longitude);
    }
}
