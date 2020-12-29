using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IDeliveryCustomerFlows
    {
        /// <summary>
        /// Return object with 3 List lookups for DeliveryCustomer Entities Types
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        DeliveryCustomerLookupModel GetLookups(DBInfoModel dbInfo);

        /// <summary>
        /// Search delivery customers in paged result with filters as flat search model 
        /// if a filter property is null or empty or does not exist it is been ignored
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="filters">Name , address , phone , trn</param>
        /// <returns>Paged result of flat lookup</returns>
        PaginationModel<DeliveryCustomerSearchModel> SearchPagedCustomersFlow(DBInfoModel dbInfo, int page, int pageSize, DeliveryCustomerFilterModel filters);

        /// <summary>
        /// Provide Id to get customer with details arrays 
        /// Assocs, Phones , Addresses billing and shipping
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id">Unique identifier of LocalDB Delivery_Customer table</param>
        /// <returns>DeliveryCustomer with details arrays</returns>
        DeliveryCustomerModel GetCustomerById(DBInfoModel Store, long Id, long PhoneId, long SAddressId, long BAddressId);

        /// <summary>
        /// Given Customer flow parses model to tasks 
        /// if model id is 0 then is switches to add Task
        /// else switches to UpdateTask
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model">Model to handle with dtos </param>
        /// <returns></returns>
        DeliveryCustomerModel UpsertCustomer(DBInfoModel Store, DeliveryCustomerModel model);

        DeliveryCustomerModel UpsertCustomerByExternalId(DBInfoModel Store, DeliveryCustomerModel model);

        /// <summary>
        /// Updates Delivery Customer by model provided 
        /// Updated Guest with new Values and selected phones and ship and bill  addresses 
        /// Uses Tasks of upsert and updateGuest
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model">Guest model </param>
        /// <returns></returns>
        GuestModel UpdateCustomerAndGuest(DBInfoModel Store, DeliveryCustomerModel model);


        /// <summary>
        /// Used for delivery service
        /// Upsert Delivery Customer by model provided Using *****ExtCustId**********
        /// UpsertsGuest with new Values and selected phones and ship and bill  addresses 
        /// Uses Tasks of upsert and updateGuest
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model">Guest model</param>
        /// <returns> Updated Guest from database </returns>
        DeliveryCustomerModelDS UpsertCustomerAndGuest(DBInfoModel Store, DeliveryCustomerModelDS model);

        /// <summary>
        /// Delete Customer by id provided 
        /// calls  delivery customer task to delete by id given
        /// Task also deletes addresses Assocs and Phones by this id as (FK) customer-id 
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="id">Id of Delivery Customer Entry</param>
        /// <returns>Id Provided for delete Functionality</returns>
        long DeleteCustomer(DBInfoModel Store, long id);
    }
}
