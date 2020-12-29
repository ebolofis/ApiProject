using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.WebApi.DataAccess.Interfaces.DAO;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.DataAccess.Interfaces.XMLs;
using Symposium_DTOs.PosModel_Info;
using System.Transactions;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IDeliveryCustomersDT
    {
        /// <summary>
        /// Return object with 3 List lookups for DeliveryCustomer Entities Types
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        DeliveryCustomerLookupModel LookupTypes(DBInfoModel Store);

        /// <summary>
        /// Provide filters to get paged flat searchModel of delivery customer
        /// Creates sql query with filters provided also a where predicate to filter results 
        /// then calls generic dao to create selection of obj asked and parse it into PaginationModel.PageList
        /// </summary>
        /// <param name="store"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageLength"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        PaginationModel<DeliveryCustomerSearchModel> SearchPagedCustomers(DBInfoModel store, int pageNumber, int pageLength, DeliveryCustomerFilterModel filters);

        /// <summary>
        /// Provide Id to get customer with details arrays 
        /// Assocs, Phones , Addresses billing and shipping
        /// </summary>
        /// <param name="store"></param>
        /// <param name="Id">Unique identifier of LocalDB Delivery_Customer table</param>
        /// <returns>DeliveryCustomer with details arrays</returns>
        DeliveryCustomerModel GetCustomerById(DBInfoModel Store, long Id, long PhoneId, long SAddressId, long BAddressId);
        DeliveryCustomerModel GetCustomerByExtKeyId(DBInfoModel Store, string ExtCUSTID, int? ExtTYPE);

        DeliveryCustomerModel AddCustomer(DBInfoModel store, DeliveryCustomerModel model);

        DeliveryCustomerModel UpdateCustomer(DBInfoModel store, DeliveryCustomerModel model);

        DeliveryCustomerModelDS UpsertCustomerWithExtCustId(DBInfoModel store, DeliveryCustomerModelDS model);

        /// <summary>
        /// Provide id of customer to delete
        /// Shippping addresses Billing Addresses , Phone and assocs 
        /// Then tryies to delete Customer with this id and returns this id else throws 
        /// exception raised 
        /// </summary>
        /// <param name="store"></param>
        /// <param name="id">id  of customer to delete and filter its deps</param>
        /// <returns>Id of Delivery Customer Deleted</returns>
        long DeleteCustomer(DBInfoModel store, long id);

        /// <summary>
        /// Get's Id From Delivery_Customers for specific ExtCustId and ExtType
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ExtId"></param>
        /// <param name="ExtType"></param>
        /// <returns></returns>
        long? GetCustomerIdByExtId(DBInfoModel Store, string ExtId, ExternalSystemOrderEnum ExtType);

    }
}
