using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_CustomerTokenDT
    {
        /// <summary>
        /// Select customer token by customer id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="customerId"></param>
        DA_CustomerTokenModel SelectCustomerToken(DBInfoModel Store, long customerId);

        /// <summary>
        /// Insert customer token
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        long InsertCustomerToken(DBInfoModel Store, DA_CustomerTokenModel model);

        /// <summary>
        /// Update customer token
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        long UpdateCustomerToken(DBInfoModel Store, DA_CustomerTokenModel model);

        /// <summary>
        /// Delete customer token
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        long DeleteCustomerToken(DBInfoModel Store, long id);

    }
}
