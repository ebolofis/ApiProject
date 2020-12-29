using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_AddressesFlows
    {
        /// <summary>
        /// Add new Address 
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="CustomerId">CustomerId from authorization header</param>
        /// <returns></returns>
         long AddAddress(DBInfoModel dbInfo, DA_AddressModel Model, long CustomerId);

        /// <summary>
        /// Update an Address 
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="CustomerId">CustomerId from authorization header</param>
        /// <returns></returns>
        long UpdateAddress(DBInfoModel dbInfo, DA_AddressModel Model, long CustomerId);

        /// <summary>
        /// Update address with phone and clear same phone from other addresses
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        void UpdateAddressPhone(DBInfoModel dbInfo, DA_AddressPhoneModel model);

        /// <summary>
        /// Delete an Address 
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">Address Id</param>
        /// <param name="CustomerId">CustomerId from authorization header</param>
        /// <returns></returns>
        long DeleteAddress(DBInfoModel dbInfo, long Id, long CustomerId);

        /// <summary>
        /// Retreive Coordinate Informations From Google or Terra Maps by giving an Address Model
        /// </summary>
        /// <param name="Model">DA_AddressModel</param>
        /// <returns>DA_AddressModel</returns>
        DA_AddressModel GeoLocationMaps(DBInfoModel dbInfo, DA_AddressModel Model);

        /// <summary>
        /// Get All Active addresses for a Customer
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">Customer Id</param>
        /// <returns></returns>
        List<DA_AddressModel> GetCustomerAddresses(DBInfoModel dbInfo, long Id);

        long GetCustomerAddressById(DBInfoModel dbinfo, long Id);
        long GetCustomerAddressByExtId(DBInfoModel dbinfo, string Id);


        /// <summary>
        /// Fill columns Phonetics and PhoneticsArea to ALL rows into table DA_Addresses with the respective phonetic values.
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns>AddressId</returns>
        void CreateAllAddressPhonetics(DBInfoModel dbInfo);
    }
}
