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
    public class DeliveryCustomersPhonesDAO: IDeliveryCustomersPhonesDAO
    {

        IGenericDAO<Delivery_CustomersPhonesDTO> genPhone;
        public DeliveryCustomersPhonesDAO(IGenericDAO<Delivery_CustomersPhonesDTO> _genPhone)
        {
            genPhone = _genPhone;
        }

        /// <summary>
        /// Function under instance to get DC Phone Addresses based on Customer Id ref
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns>List Of Phone Addresses DB models filtered on Customer Id </returns>
        public List<Delivery_CustomersPhonesDTO> SelectDCustomerPhoneByCustomerId(IDbConnection db, long Id)
        {
            return genPhone.Select(db, "where CustomerId = @CustomerId ", new { CustomerId = Id });
        }

        /// <summary>
        /// Function that returns Phone Addreses based on external Key 
        /// If Exttype is provided then is applyies an AND filter to select Addresses with ExtKey and Type of ExternalSystemOrderEnum
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ExtKey"> Key of entity from an external system  </param>
        /// <param name="ExtType"> ExternalSystemOrderEnum </param>
        /// <returns></returns>
        public List<Delivery_CustomersPhonesDTO> SelectDCustomerPhoneByExtId(IDbConnection db, string ExtKey, int? ExtType = null)
        {
            string whereq = "where ExtKey = @Key ";
            if (ExtType != null)
            {
                whereq += "and ExtType = @EType";
            }
            return genPhone.Select(db, whereq, new { Key = ExtKey, EType = ExtType });
        }
    }
}
