using Symposium.Models.Enums;
using Symposium.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IDeliveryCustomerTasks
    {
        /// <summary>
        /// Return object with 3 List lookups for DeliveryCustomer Entities Types
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        DeliveryCustomerLookupModel GetDeliveryLookups(DBInfoModel Store);
        /// <summary>
        /// Search delivery customers in paged result with filters as flat search model 
        /// if a filter property is null or empty or does not exist it is been ignored
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="filters">Name , address , phone , trn</param>
        /// <returns>Paged result of flat lookup</returns>
        PaginationModel<DeliveryCustomerSearchModel> SearchPagedCustomersTask(DBInfoModel store, int page, int pageSize, DeliveryCustomerFilterModel filters);

        /// <summary>
        /// Provide Id to get customer with details arrays 
        /// Assocs, Phones , Addresses billing and shipping
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id">Unique identifier of LocalDB Delivery_Customer table</param>
        /// <returns>DeliveryCustomer with details arrays</returns>
        DeliveryCustomerModel GetCustomerById(DBInfoModel Store, long Id, long PhoneId, long SAddressId, long BAddressId);

        /// <summary>
        /// Provide ExtCUSTID and or TYPE to get customer with details arrays 
        /// Assocs, Phones , Addresses billing and shipping
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ExtCUSTID"> External Key identification of system stored customer </param>
        /// <param name="ExtTYPE"> EXt Type of System</param>
        /// <returns>DeliveryCustomer with details arrays</returns>
        DeliveryCustomerModel GetCustomerByExtKeyId(DBInfoModel Store, string ExtCUSTID, int? ExtTYPE);

        /// <summary>
        /// Case when moden from flow parsed with id 0 
        /// uses data access layer to add logic 
        /// adds model then returns new model with id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model">Model of DeliveryCustomer to Add on DB</param>
        /// <returns> New Added Model </returns>
        DeliveryCustomerModel InsertCustomerTask(DBInfoModel Store, DeliveryCustomerModel model);

        /// <summary>
        /// Case when moden from flow parsed with id != 0 
        /// uses data access layer to update logic 
        /// adds model then returns new model updated
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model">Model of DeliveryCustomer to Add on DB</param>
        /// <returns> New Added Model </returns>
        DeliveryCustomerModel UpdateCustomerTask(DBInfoModel Store, DeliveryCustomerModel model);

        long? FindCustomerByExternalId(DBInfoModel Store, string externalId, ExternalSystemOrderEnum externalType);

        /// <summary>
        /// Upsert DeliveryCustomer model with model provided using ExtCustId
        /// Used from Delivery Service to keep functionality save with POS system
        /// Calls with return GuestDataAccess and updates Guest with new vals ,  selected addresses and phones
        /// Returns updated Guest
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model">DeliveryCustomerModel to update model and Guest</param>
        /// <returns>DeliveryCustomerModelDS Model updated from Customer with guest ID from updated Guest </returns>
        DeliveryCustomerModelDS UpsertCustomerAndGuest(DBInfoModel Store, DeliveryCustomerModelDS model);


        /// <summary>
        /// Updates DeliveryCustomer model with model provided
        /// Calls with return GuestDataAccess and updates Guest with new vals ,  selected addresses and phones
        /// Returns updated Guest
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model">DeliveryCustomerModel to update model and Guest</param>
        /// <returns>Guest Model updated from Customer</returns>
        GuestModel UpdateCustomerAndGuest(DBInfoModel Store, DeliveryCustomerModel model);

        /// <summary>
        /// Calls data access layer with store to create dbcontext instance and deletes 
        /// Delivery Customer Entities mapped with customer id provided
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="id">Id of Delivery Customer Entry</param>
        /// <returns>Id Provided for delete Functionality</returns>
        long DeleteCustomerTask(DBInfoModel Store, long id);
    }
}
