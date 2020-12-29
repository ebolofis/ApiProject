using Symposium.Models.Models;
using Symposium.Models.Models.ExternalSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.Goodys
{
    public interface IGoodysOrdersFlow
    {
        /// <summary>
        /// Return's a Login responce model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="AccountId"></param>
        /// <param name="allAddresses"></param>
        /// If Param allAddress = true then shipping and billing addresses returned else only shipping addresses.
        /// Deleted addresses never returned
        /// <returns></returns>
        GoodysLoginResponceModel GetLoginResponceModel(DBInfoModel dbInfo, string AccountId, bool allAddresses = true);

        /// <summary>
        /// Inserts new customer and address on da_customer and da_address table based on a Goodys Registration Model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        GoodysLoginResponceModel RegisterCustomer(DBInfoModel dbInfo, GoodysRegisterModel model);

        /// <summary>
        /// Create's new Address to customer
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        GoodysLoginResponceModel CreateNewAddress(DBInfoModel dbInfo, GoodysLoginAddressResponceModel model);

        /// <summary>
        /// Deletes an address of a specific customer
        /// </summary>
        /// <param name="dBInfo"></param>
        /// <param name="addressId"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        GoodysLoginResponceModel DeleteAddress(DBInfoModel dBInfo, string addressId, string accountId);

        /// <summary>
        /// Insert an order to da_orders and returns the given model improved with 3 fields (orderNo, CustomerId and Store)
        /// </summary>
        /// <param name="dBInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        GoodysDA_OrderModel InsertOrder(DBInfoModel dBInfo, GoodysDA_OrderModel model);
    }
}
