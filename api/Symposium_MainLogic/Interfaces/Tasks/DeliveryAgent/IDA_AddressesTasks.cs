using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent
{
    public interface IDA_AddressesTasks
    {
        /// <summary>
        /// Get All Active addresses for a Customer
        /// </summary>
        /// <param name="dbInfo">db</param>
        /// <param name="Id">Customer Id</param>
        /// <returns></returns>
        List<DA_AddressModel> getCustomerAddresses(DBInfoModel dbInfo, long Id);


        /// <summary>
        /// Add new Address 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long AddAddress(DBInfoModel dbInfo, DA_AddressModel Model);

        /// <summary>
        /// Update an Address 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long UpdateAddress(DBInfoModel dbInfo, DA_AddressModel Model);

        /// <summary>
        /// Delete Address OR set the IsDeleted = 1. If address is deleted then return 1, if set IsDeleted = 1 then return 0
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        long DeleteAddress(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// check if DA_AddressesModels contains the correct customerId. If not then trow exception
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="CustomerId"></param>
        void CheckOwner(DA_AddressModel Model, long CustomerId);

        /// <summary>
        /// Retreive Coordinate Informations From Google or Terra Maps by giving an Address Model
        /// </summary>
        /// <param name="Model">DA_AddressModel</param>
        /// <returns>DA_AddressModel</returns>
        DA_AddressModel GeoLocationMaps(DBInfoModel dbInfo, DA_AddressModel Model);

        long GetCustomerAddressById(DBInfoModel dbinfo, long Id);
        long GetCustomerAddressByExtId(DBInfoModel dbinfo, string Id);

        /// <summary>
        /// Change OwnerId
        /// </summary>
        /// <param name="Id">DA_Addresses Id</param>
        /// <param name="OwnerId">OwnerId</param>
        /// <returns>AddressId</returns>
        long ChangeOwnerId(DBInfoModel dbInfo, long Id, long OwnerId);

        /// <summary>
        /// Fill columns Phonetics and PhoneticsArea to ALL rows into table DA_Addresses with the respective phonetic values.
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns>AddressId</returns>
        void CreateAllAddressPhonetics(DBInfoModel dbInfo);

    }
}
