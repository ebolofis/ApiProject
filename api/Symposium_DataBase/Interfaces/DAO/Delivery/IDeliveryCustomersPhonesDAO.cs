﻿using Symposium_DTOs.PosModel_Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DAO.Delivery
{
    public interface IDeliveryCustomersPhonesDAO
    {
        /// <summary>
        /// Function under instance to get DC Phone Addresses based on Customer Id ref
        /// </summary>
        /// <param name="db"></param>
        /// <param name="Id"></param>
        /// <returns>List Of Phone Addresses DB models filtered on Customer Id </returns>
        List<Delivery_CustomersPhonesDTO> SelectDCustomerPhoneByCustomerId(IDbConnection db, long Id);

        /// <summary>
        /// Function that returns Phone Addreses based on external Key 
        /// If Exttype is provided then is applyies an AND filter to select Addresses with ExtKey and Type of ExternalSystemOrderEnum
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ExtKey"> Key of entity from an external system  </param>
        /// <param name="ExtType"> ExternalSystemOrderEnum </param>
        /// <returns></returns>
        List<Delivery_CustomersPhonesDTO> SelectDCustomerPhoneByExtId(IDbConnection db, string ExtKey, int? ExtType = null);
    }
}
