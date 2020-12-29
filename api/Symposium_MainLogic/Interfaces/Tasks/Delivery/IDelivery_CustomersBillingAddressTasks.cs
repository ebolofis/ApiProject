using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IDelivery_CustomersBillingAddressTasks
    {

        /// <summary>
        /// Return's a Delivery Billing Nodel using External Key
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CustomerId"></param>
        /// <param name="ExtKey"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        Delivery_CustomersBillingAddressModel GetModelByExternalKey(DBInfoModel Store, long CustomerId, string ExtKey, ExternalSystemOrderEnum ExtType);

        DeliveryCustomersBillingAddressModel GetAddressByLatLng(DBInfoModel Store, string latitude, string longitude);
    }
}
