using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_StoresDT
    {
        /// <summary>
        /// Get a List of Stores
        /// </summary>
        /// <returns>DA_StoreModel</returns>
        List<DA_StoreModel> GetStores(DBInfoModel dbInfo);

        /// <summary>
        /// Get a List of Stores With Latitude and Longtitude
        /// </summary>
        /// <returns>DA_StoreInfoModel</returns>
        List<DA_StoreInfoModel> GetStoresPosition(DBInfoModel dbInfo);

        /// <summary>
        /// Get A Specific Order
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        DA_StoreInfoModel GetStoreById(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Return Store Id based on Store Code. If no Code found then return 0.
        /// </summary>
        /// <param name="dbInfo">dbInfo</param>
        /// <param name="Code">Store Code</param>
        /// <returns></returns>
        long? GetStoreIdFromCode(DBInfoModel dbInfo, string Code);


        /// <summary>
        /// insert new DA Store. Return new Id
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="StoreModel">DA_StoreModel</param>
        /// <returns></returns>
        long Insert(DBInfoModel dbInfo, DA_StoreModel StoreModel);

        /// <summary>
        /// update a DA Store. Return number of rows affected
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="StoreModel">DA_StoreModel</param>
        /// <returns></returns>
        long Update(DBInfoModel dbInfo, DA_StoreModel StoreModel);

        /// <summary>
        /// Update DA_Store Set Notes to NUll
        /// <param name="StoreId"></param>
        /// </summary>
        long UpdateDaStoreNotes(DBInfoModel dbInfo, long StoreId);


        /// <summary>
        /// delete a DA Store. Return number of rows affected. 
        /// Also DELETE from DA_ShortageProds, DA_ScheduleTaskes, DA_PriceListAssoc, DA_Addresses
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="Id">DA_Store.Id</param>
        /// <returns></returns>
        long Delete(DBInfoModel dbInfo, long Id);

        long BODelete(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Update Store's DeliveryTime, TakeOutTime, StoreStatus
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="daStoreId">DA_Stores.Id</param>
        /// <param name="deliveryTime">deliveryTime (min)</param>
        /// <param name="takeOutTime">takeOutTime (min)</param>
        /// <param name="storeStatus">storeStatus</param>
        void UpdateTimesStatus(DBInfoModel dbInfo, long daStoreId, int deliveryTime, int takeOutTime, DAStoreStatusEnum storeStatus);
}

}