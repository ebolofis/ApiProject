using Geocoding;
using Geocoding.Google;
using Symposium.Helpers.Interfaces;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class DeliveryCustomerTasks : IDeliveryCustomerTasks
    {

        IDeliveryCustomersDT delcustDB;
        IGuestDT guestDB;
        IDA_AddressesTasks addressTasks;

        public DeliveryCustomerTasks(IDeliveryCustomersDT _delcustDB, IGuestDT _guestDB,  IDA_AddressesTasks addressTasks)
        {
            this.delcustDB = _delcustDB;
            this.guestDB = _guestDB;
            this.addressTasks = addressTasks;
        }

        /// <summary>
        /// Return object with 3 List lookups for DeliveryCustomer Entities Types
        /// </summary>
        /// <param name="Store"></param>
        /// <returns></returns>
        public DeliveryCustomerLookupModel GetDeliveryLookups(DBInfoModel Store)
        {
            return delcustDB.LookupTypes(Store);
        }

        /// <summary>
        /// Search delivery customers in paged result with filters as flat search model 
        /// if a filter property is null or empty or does not exist it is been ignored
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="filters">Name , address , phone , trn</param>
        /// <returns>Paged result of flat lookup</returns>
        public PaginationModel<DeliveryCustomerSearchModel> SearchPagedCustomersTask(DBInfoModel store, int page, int pageSize, DeliveryCustomerFilterModel filters)
        {
            return delcustDB.SearchPagedCustomers(store, page, pageSize, filters);
        }

        /// <summary>
        /// Provide Id to get customer with details arrays 
        /// Assocs, Phones , Addresses billing and shipping
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id">Unique identifier of LocalDB Delivery_Customer table</param>
        /// <returns>DeliveryCustomer with details arrays</returns>
        public DeliveryCustomerModel GetCustomerById(DBInfoModel Store, long Id, long PhoneId, long SAddressId, long BAddressId)
        {
            return delcustDB.GetCustomerById(Store, Id, PhoneId, SAddressId, BAddressId);
        }

        /// <summary>
        /// Provide ExtCUSTID and or TYPE to get customer with details arrays 
        /// Assocs, Phones , Addresses billing and shipping
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="ExtCUSTID"> External Key identification of system stored customer </param>
        /// <param name="ExtTYPE"> EXt Type of System </param>
        /// <returns> DeliveryCustomer with details arrays </returns>
        public DeliveryCustomerModel GetCustomerByExtKeyId(DBInfoModel Store, string ExtCUSTID, int? ExtTYPE)
        {
            return delcustDB.GetCustomerByExtKeyId(Store, ExtCUSTID, ExtTYPE);
        }

        /// <summary>
        /// Case when moden from flow parsed with id 0 
        /// uses data access layer to add logic 
        /// adds model then returns new model with id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model">Model of DeliveryCustomer to Add on DB</param>
        /// <returns> New Added Model </returns>
        public DeliveryCustomerModel InsertCustomerTask(DBInfoModel Store, DeliveryCustomerModel model)
        {
            DA_AddressModel pluginAdr = new DA_AddressModel();
            DA_AddressModel daAddr = null;

            if (model.ShippingAddresses != null)
            {
                foreach (DeliveryCustomersShippingAddressModel shipping in model.ShippingAddresses)
                {
                    pluginAdr.AddressStreet = shipping.AddressStreet;
                    pluginAdr.AddressNo = (shipping.AddressNo != "" ? (" " + shipping.AddressNo) : "");
                    pluginAdr.City = (shipping.City != "" ? (" " + shipping.City) : "");
                    pluginAdr.Area = (shipping.City != "" ? (" " + shipping.City) : "");
                    pluginAdr.Zipcode = (shipping.Zipcode != "" ? (" " + shipping.Zipcode) : "");
                    pluginAdr.ExtId1 = shipping.ExtId1;
                    pluginAdr.Floor = shipping.Floor;
                    daAddr = addressTasks.GeoLocationMaps(Store, pluginAdr);
                    shipping.Latitude = daAddr.Latitude.ToString();
                    shipping.Longtitude = daAddr.Longtitude.ToString();
                }
            }
            if (model.BillingAddresses != null)
            {
                foreach (DeliveryCustomersBillingAddressModel billing in model.BillingAddresses)
                {
                    pluginAdr.AddressStreet = billing.AddressStreet;
                    pluginAdr.AddressNo = (billing.AddressNo != "" ? (" " + billing.AddressNo) : "");
                    pluginAdr.City = (billing.City != "" ? (" " + billing.City) : "");
                    pluginAdr.Area = (billing.City != "" ? (" " + billing.City) : "");
                    pluginAdr.Zipcode = (billing.Zipcode != "" ? (" " + billing.Zipcode) : "");
                    pluginAdr.ExtId1 = billing.ExtId1;
                    pluginAdr.Floor = billing.Floor;
                    daAddr = addressTasks.GeoLocationMaps(Store, pluginAdr);
                    billing.Latitude = daAddr.Latitude.ToString();
                    billing.Longtitude = daAddr.Longtitude.ToString();
                }
            }

            return delcustDB.AddCustomer(Store, model);
        }

               /// <summary>
        /// Case when moden from flow parsed with id != 0 
        /// uses data access layer to update logic 
        /// adds model then returns new model updated
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model">Model of DeliveryCustomer to Add on DB</param>
        /// <returns> New Added Model </returns>
        public DeliveryCustomerModel UpdateCustomerTask(DBInfoModel Store, DeliveryCustomerModel model)
        {
            return delcustDB.UpdateCustomer(Store, model);
        }

        public long? FindCustomerByExternalId(DBInfoModel Store, string externalId, ExternalSystemOrderEnum externalType)
        {
            return delcustDB.GetCustomerIdByExtId(Store, externalId, externalType);
        }

        /// <summary>
        /// Updates DeliveryCustomer model with model provided
        /// Calls with return GuestDataAccess and updates Guest with new vals ,  selected addresses and phones
        /// Returns updated Guest
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model">DeliveryCustomerModel to update model and Guest</param>
        /// <returns>Guest Model updated from Customer</returns>
        public GuestModel UpdateCustomerAndGuest(DBInfoModel Store, DeliveryCustomerModel model)
        {
            //return null;
            DeliveryCustomerModel updated = delcustDB.UpdateCustomer(Store, model);
            return guestDB.UpdateGuestFromDeliveryCustomer(Store, updated);
        }

        /// <summary>
        /// Upsert DeliveryCustomer model with model provided using ExtCustId
        /// Used from Delivery Service to keep functionality save with POS system
        /// Calls with return GuestDataAccess and updates Guest with new vals ,  selected addresses and phones
        /// Returns updated Guest
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model">DeliveryCustomerModel to update model and Guest</param>
        /// <returns>DeliveryCustomerModelDS Model updated from Customer with guest ID from updated Guest </returns>
        public DeliveryCustomerModelDS UpsertCustomerAndGuest(DBInfoModel Store, DeliveryCustomerModelDS model)
        {
            return delcustDB.UpsertCustomerWithExtCustId(Store, model);
        }


        /// <summary>
        /// Calls data access layer with store to create dbcontext instance and deletes 
        /// Delivery Customer Entities mapped with customer id provided
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="id">Id of Delivery Customer Entry</param>
        /// <returns>Id Provided for delete Functionality</returns>
        public long DeleteCustomerTask(DBInfoModel Store, long id)
        {
            return delcustDB.DeleteCustomer(Store, id);
        }
    }
}
