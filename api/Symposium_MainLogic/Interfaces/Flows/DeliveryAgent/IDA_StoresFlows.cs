using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_StoresFlows
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
        /// Get A Specific Store based on Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        DA_StoreModel GetStoreById(DBInfoModel dbInfo, long Id);

        /// <summary>
        /// Get A Specific Store based on StaffId. Κλήση από το κατάστημα μόνο.
        /// </summary>
        /// <param name = "dbInfo" > DB con string</param>
        /// <param name="staffId">staff.Id</param>
        /// <returns></returns>
        DA_StoreInfoModel GetStoreByStaffId(DBInfoModel dbInfo, long staffId);

        /// <summary>
        /// Update DA_Store Set Notes to NUll
        /// <param name="StoreId"></param>
        /// </summary>
        long UpdateDaStoreNotes(DBInfoModel dbInfo, long StaffId);

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
        /// delete a DA Store. Return number of rows affected. 
        /// Also DELETE from DA_ShortageProds, DA_ScheduleTaskes, DA_PriceListAssoc, DA_Addresses
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="Id">DA_Store.Id</param>
        /// <returns></returns>
        long Delete(DBInfoModel dbInfo, long Id);

        long BODelete(DBInfoModel DBInfo, long Id);

        /// <summary>
        /// Update Store's DeliveryTime, TakeOutTime, StoreStatus
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="staffId">Staff.Id</param>
        /// <param name="deliveryTime">deliveryTime (min)</param>
        /// <param name="takeOutTime">takeOutTime (min)</param>
        /// <param name="storeStatus">storeStatus</param>
        void UpdateTimesStatus(DBInfoModel dbInfo, long staffId, int deliveryTime, int takeOutTime, DAStoreStatusEnum storeStatus);

        /// <summary>
        /// Updates Specific Store's Tables whith Server Data
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="StoreId"></param>
        void UpdateClientStore(DBInfoModel dbInfo, long StoreId);
    }
}
