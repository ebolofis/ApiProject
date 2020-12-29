using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DAO.Delivery;
using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.DAOs.Delivery
{
    public class DeliveryCustomersShippingAddressDAO : IDeliveryCustomersShippingAddressDAO
    {
        IGenericDAO<Delivery_CustomersShippingAddressDTO> genShipAddress;
        public DeliveryCustomersShippingAddressDAO(IGenericDAO<Delivery_CustomersShippingAddressDTO> _genShipAddress)
        {
            genShipAddress = _genShipAddress;
        }

        /// <summary>
        /// Function under instance to get DC Shipping Addresses based on Customer Id ref
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns> List Of Shipping Addresses DB models filtered on Customer Id </returns>
        public List<Delivery_CustomersShippingAddressDTO> SelectDCustomerShippingAddressByCustomerId(IDbConnection db, long Id)
        {
            return genShipAddress.Select(db, "where CustomerId = @CustomerId and isnull(IsDeleted , 0)=0 ", new { CustomerId = Id });
        }


        /// <summary>
        /// Function that returns Shipping Addreses based on external Key 
        /// If Exttype is provided then is applyies an AND filter to select Addresses with ExtKey and Type of ExternalSystemOrderEnum
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ExtKey"> Key of entity from an external system  </param>
        /// <param name="ExtType"> ExternalSystemOrderEnum </param>
        /// <returns></returns>
        public List<Delivery_CustomersShippingAddressDTO> SelectDCustomerShippingAddressByExtId(IDbConnection db, string ExtKey, int? ExtType = null)
        {
            string whereq = "where ExtKey = @Key and isnull(IsDeleted , 0)=0 ";
            if (ExtType != null)
            {
                whereq += "and ExtType = @EType";
            }
            return genShipAddress.Select(db, whereq, new { Key = ExtKey, EType = ExtType });
        }

    }
}
