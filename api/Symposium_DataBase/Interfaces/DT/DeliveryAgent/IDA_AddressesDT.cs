using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_AddressesDT
    {

        /// <summary>
        /// return an address based on Id
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="Id">Address Id</param>
        /// <returns></returns>
        DA_AddressModel getAddress(DBInfoModel Store, long Id);

        /// <summary>
        /// Get All addresses (billing & shipping) for a Customer
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">Customer Id</param>
        /// <returns></returns>
        List<DA_AddressModel> getCustomerAddresses(DBInfoModel Store, long Id);

        /// <summary>
        /// from customer's addresses return the one that is far from a certain point less than x meters.
        /// </summary>
        /// <param name="Store">db</param>
        /// <param name="Id">Customer Id</param>
        /// <param name="Latitude">certain point's Latitude</param>
        /// <param name="Longtitude">certain point's Longitude</param>
        /// <param name="Distance">the maximum distance in meters (default value 32m)</param>
        /// <returns></returns>
        DA_AddressModel proximityCustomerAddress(DBInfoModel Store, long Id, float Latitude, float Longtitude, int Distance = 32);

        /// <summary>
        /// Add new Address 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long AddAddress(DBInfoModel Store, DA_AddressModel Model);

        /// <summary>
        /// Update an Address 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        long UpdateAddress(DBInfoModel Store, DA_AddressModel Model);

        /// <summary>
        /// Delete Address OR set the IsDeleted = 1. If address is deleted then return 1, if set IsDeleted = 1 then return 0
        /// </summary>
        /// <param name="Id">Address id</param>
        /// <returns></returns>
        long DeleteAddress(DBInfoModel Store, long Id);

        long GetCustomerAddressById(DBInfoModel dbinfo, long Id);
        long GetCustomerAddressByExtId(DBInfoModel dbinfo, string ExtId2);

        /// <summary>
        /// Change OwnerId
        /// </summary>
        /// <param name="Id">DA_Addresses Id</param>
        /// <param name="OwnerId">OwnerId</param>
        /// <returns>AddressId</returns>
        long ChangeOwnerId(DBInfoModel dbinfo, long Id, long OwnerId);
    }
}
