using log4net;
using Symposium.Helpers;
using Symposium.Models.Enums;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Scheduler;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_StoresFlows: IDA_StoresFlows
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        IDA_StoresTasks storeTasks;
        IDA_OrdersTasks ordersTasks;
        IStaffTasks staffTasks;
        IDA_UpdateStoresTablesTasks updateClientTask; 

        public DA_StoresFlows(IDA_StoresTasks _storeTasks, IDA_OrdersTasks ordersTasks, IStaffTasks staffTasks,
            IDA_UpdateStoresTablesTasks updateClientTask)
        {
            this.storeTasks = _storeTasks;
            this.ordersTasks = ordersTasks;
            this.staffTasks = staffTasks;
            this.updateClientTask = updateClientTask;
        }

        /// <summary>
        /// Get a List of Stores
        /// </summary>
        /// <returns>DA_StoreModel</returns>
        public List<DA_StoreModel> GetStores(DBInfoModel dbInfo)
        {
            return storeTasks.GetStores(dbInfo);
        }

        /// <summary>
        /// Get a List of Stores With Latitude and Longtitude
        /// </summary>
        /// <returns>DA_StoreInfoModel</returns>
        public List<DA_StoreInfoModel> GetStoresPosition(DBInfoModel dbInfo)
        {
            return storeTasks.GetStoresPosition(dbInfo);
        }

        /// <summary>
        /// Get A Specific Store based on Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public DA_StoreModel GetStoreById(DBInfoModel dbInfo, long Id)
        {
            return storeTasks.GetStoreById(dbInfo, Id);
        }


        /// <summary>
        /// Get A Specific Store based on StaffId. Κλήση από το κατάστημα μόνο.
        /// </summary>
        /// <param name = "dbInfo" > DB con string</param>
        /// <param name="staffId">staff.Id</param>
        /// <returns></returns>
        public DA_StoreInfoModel GetStoreByStaffId(DBInfoModel dbInfo, long staffId)
        {

            //1. Get store Id (if there is no store then throw exception)
            long daStoreId = staffTasks.GetDaStore(dbInfo, staffId);
            if (daStoreId == 0) throw new BusinessException(Symposium.Resources.Errors.STAFFNOASSIGNED);

            //2. get Store
            return storeTasks.GetStoreById(dbInfo, daStoreId);
        }

        /// <summary>
        /// Update DA_Store Set Notes to NUll
        /// <param name="StoreId"></param>
        /// </summary>
        public long UpdateDaStoreNotes(DBInfoModel dbInfo, long StaffId)
        {
            //1. Get store Id (if there is no store then throw exception)
            long daStoreId = staffTasks.GetDaStore(dbInfo, StaffId);
            if (daStoreId == 0) throw new BusinessException(Symposium.Resources.Errors.STAFFNOASSIGNED);


            return storeTasks.UpdateDaStoreNotes(dbInfo, daStoreId);
        }


        /// <summary>
        /// insert new DA Store. Return new Id
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="StoreModel">DA_StoreModel</param>
        /// <returns></returns>
        public long Insert(DBInfoModel dbInfo, DA_StoreModel StoreModel)
        {
            //1. insert the new Store to DB
            StoreModel.Id = 0;
            long id= storeTasks.Insert(dbInfo, StoreModel);
           
            //2. DB Trigger add to DA_ScheduleTaskes all records for products, price lists, etc...

            return id;
        }


        /// <summary>
        /// update a DA Store. Return number of rows affected
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="StoreModel">DA_StoreModel</param>
        /// <returns></returns>
        public long Update(DBInfoModel dbInfo, DA_StoreModel StoreModel)
        {
            //1. update
            return storeTasks.Update(dbInfo, StoreModel);

            //2. DB Trigger check if store connection properties have changed....
        }

        /// <summary>
        /// delete a DA Store. Return number of rows affected
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="Id">DA_Store.Id</param>
        /// <returns></returns>
        public long Delete(DBInfoModel dbInfo, long Id)
        {
          DA_StoreModel storeModel=  storeTasks.GetStoreById(dbInfo,Id);
            //1. check if store has orders.
            int orders=ordersTasks.GetStoreOrderNo(dbInfo, Id);
            if (orders > 0) throw new BusinessException("Store cannot be deleted because Orders have been made. Change store's status to Inactive instead. ");
            //2. delete store
            return storeTasks.Delete(dbInfo, Id);
        }
        public long BODelete(DBInfoModel DBInfo, long Id)
        {
            DA_StoreModel storeModel = storeTasks.GetStoreById(DBInfo, Id);
            //1. check if store has orders.
            int orders = ordersTasks.GetStoreOrderNo(DBInfo, Id);
            if (orders > 0) throw new BusinessException("Store cannot be deleted because Orders have been made. Change store's status to Inactive instead. ");
            //2. delete store
            return storeTasks.BODelete(DBInfo, Id);
        }

        /// <summary>
        /// Update Store's DeliveryTime, TakeOutTime, StoreStatus
        /// </summary>
        /// <param name="dbInfo">DB con string</param>
        /// <param name="staffId">Staff.Id</param>
        /// <param name="deliveryTime">deliveryTime (min)</param>
        /// <param name="takeOutTime">takeOutTime (min)</param>
        /// <param name="storeStatus">storeStatus</param>
        public void UpdateTimesStatus(DBInfoModel dbInfo, long staffId, int deliveryTime, int takeOutTime, DAStoreStatusEnum storeStatus)
        {
            //1. Get store Id (if there is no store then throw exception)
            long daStoreId= staffTasks.GetDaStore(dbInfo, staffId);
            if (daStoreId == 0) throw new BusinessException(Symposium.Resources.Errors.STAFFNOASSIGNED);

            //2. update...
            storeTasks.UpdateTimesStatus(dbInfo, daStoreId, deliveryTime, takeOutTime, storeStatus);
        }

        /// <summary>
        /// Updates Specific Store's Tables whith Server Data
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="StoreId"></param>
        public void UpdateClientStore(DBInfoModel dbInfo, long StoreId)
        {
            try
            {
                logger.Error("Start updating tables for Store Id : " + StoreId.ToString());
                long delAfterFaildRaw = MainConfigurationHelper.GetSubConfiguration(MainConfigurationHelper.apiDeliveryConfiguration, "DA_DeleteAfterFailed");
                int delAfterFaild = Convert.ToInt32(delAfterFaildRaw);
                updateClientTask.UpdateTables(dbInfo, delAfterFaild, StoreId);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateClientStore : " + ex.ToString());
            }
            finally
            {
                logger.Error("Updating tables for Store Id : " + StoreId.ToString() + "  Completed....");
            }
        }
    }
}
