using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_AddressesFlows : IDA_AddressesFlows
    {
        IDA_AddressesTasks addressesTasks;
        public DA_AddressesFlows(IDA_AddressesTasks _addressesTasks)
        {
            this.addressesTasks = _addressesTasks;
        }

        /// <summary>
        /// Add new Address 
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="CustomerId">CustomerId from authorization header</param>
        /// <returns></returns>
        public long AddAddress(DBInfoModel dbInfo, DA_AddressModel Model, long CustomerId)
        {

            Model.Id = 0;
            addressesTasks.CheckOwner(Model, CustomerId);
            return addressesTasks.AddAddress(dbInfo, Model);
        }

        /// <summary>
        /// Update an Address 
        /// </summary>
        /// <param name="Model"></param>
        ///  <param name="CustomerId">CustomerId from authorization header</param>
        /// <returns></returns>
        public long UpdateAddress(DBInfoModel dbInfo, DA_AddressModel Model, long CustomerId)
        {
            addressesTasks.CheckOwner(Model, CustomerId);
            return addressesTasks.UpdateAddress(dbInfo, Model);
        }

        /// <summary>
        /// Update address with phone and clear same phone from other addresses
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        public void UpdateAddressPhone(DBInfoModel dbInfo, DA_AddressPhoneModel model)
        {
            List<DA_AddressModel> customerAddresses = addressesTasks.getCustomerAddresses(dbInfo, model.CustomerId);
            if (customerAddresses != null)
            {
                foreach (DA_AddressModel address in customerAddresses)
                {
                    if (address.Id.Equals(model.AddressId))
                    {
                        address.LastPhoneNumber = model.PhoneNumber;
                        long addressId = addressesTasks.UpdateAddress(dbInfo, address);
                    }
                    else if (address.LastPhoneNumber != null && address.LastPhoneNumber.Equals(model.PhoneNumber))
                    {
                        address.LastPhoneNumber = null;
                        long addressId = addressesTasks.UpdateAddress(dbInfo, address);
                    }
                }
            }
        }

        /// <summary>
        /// Delete an Address 
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">Address Id</param>
        /// <param name="CustomerId">CustomerId from authorization header</param>
        /// <returns></returns>
        public long DeleteAddress(DBInfoModel dbInfo, long Id, long CustomerId)
        {
            if (CustomerId != 0) // check if Address Id belongs to the current customer
            {
                List<DA_AddressModel> models = addressesTasks.getCustomerAddresses(dbInfo, CustomerId);
                if (models == null || models.Count == 0) return 0;
                addressesTasks.CheckOwner(models.FirstOrDefault(x => x.Id == Id), CustomerId);
            }

            return addressesTasks.DeleteAddress(dbInfo, Id);
        }

        /// <summary>
        /// Retreive Coordinate Informations From Google or Terra Maps by giving an Address Model
        /// </summary>
        /// <param name="Model">DA_AddressModel</param>
        /// <returns>DA_AddressModel</returns>
        public DA_AddressModel GeoLocationMaps(DBInfoModel dbInfo, DA_AddressModel Model)
        {
            return addressesTasks.GeoLocationMaps(dbInfo, Model);
        }

        /// <summary>
        /// Get All Active addresses for a Customer
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">Customer Id</param>
        /// <returns></returns>
        public List<DA_AddressModel> GetCustomerAddresses(DBInfoModel dbInfo, long Id)
        {
            return addressesTasks.getCustomerAddresses(dbInfo, Id);
        }

        public long GetCustomerAddressById(DBInfoModel dbinfo, long Id)
        {
            return addressesTasks.GetCustomerAddressById(dbinfo, Id);
        }

        public long GetCustomerAddressByExtId(DBInfoModel dbinfo, string ExtId2)
        {
            return addressesTasks.GetCustomerAddressByExtId(dbinfo, ExtId2);
        }

        /// <summary>
        /// Fill columns Phonetics and PhoneticsArea to ALL rows into table DA_Addresses with the respective phonetic values.
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns>AddressId</returns>
        public void CreateAllAddressPhonetics(DBInfoModel dbInfo)
        {
            addressesTasks.CreateAllAddressPhonetics(dbInfo);
        }
    }
}

