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
    public class Delivery_CustomersShippingAddressTasks : IDelivery_CustomersShippingAddressTasks        
    {
        IDelivery_CustomersShippingAddressDT dt;

        public Delivery_CustomersShippingAddressTasks(IDelivery_CustomersShippingAddressDT dt)
        {
            this.dt = dt;
        }

        /// <summary>
        /// Return's A Model bases on an External Key and External Type
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="CustomerId"></param>
        /// <param name="ExternalKey"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        public Delivery_CustomersShippingAddressModel GetModelByExternalKey(DBInfoModel Store, long CustomerId, string ExternalKey, ExternalSystemOrderEnum ExtType)
        {
            return dt.GetModelByExternalKey(Store, CustomerId, ExternalKey, ExtType);
        }

        public DeliveryCustomersShippingAddressModel GetAddressByLatLng(DBInfoModel Store, string latitude, string longitude)
        {
            return dt.GetAddressByLatLng(Store, latitude, longitude);
        }

    }
}
