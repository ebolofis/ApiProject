using Symposium.Models.Enums;
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
    public class Delivery_CustomersBillingAddressTasks : IDelivery_CustomersBillingAddressTasks
    {
        IDelivery_CustomersBillingAddressDT dt;

        public Delivery_CustomersBillingAddressTasks(IDelivery_CustomersBillingAddressDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Return's a Delivery Billing Nodel using External Key
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="CustomerId"></param>
        /// <param name="ExtKey"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public Delivery_CustomersBillingAddressModel GetModelByExternalKey(DBInfoModel dbInfo, long CustomerId, string ExtKey, ExternalSystemOrderEnum ExtType)
        {
            return dt.GetModelByExternalKey(dbInfo, CustomerId, ExtKey, ExtType);
        }

        public DeliveryCustomersBillingAddressModel GetAddressByLatLng(DBInfoModel Store, string latitude, string longitude)
        {
            return dt.GetAddressByLatLng(Store, latitude, longitude);
        }

    }
}
