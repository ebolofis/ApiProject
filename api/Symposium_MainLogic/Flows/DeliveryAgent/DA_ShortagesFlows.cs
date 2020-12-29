using Symposium.Helpers;
using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.DeliveryAgent;
using Symposium.WebApi.MainLogic.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.DeliveryAgent
{
    public class DA_ShortagesFlows : IDA_ShortagesFlows
    {
        IDA_ShortagesTasks shortagesTasks;
        IStaffTasks staffTasks;

        public DA_ShortagesFlows(IDA_ShortagesTasks _shortagesTasks, IStaffTasks staffTasks)
        {
            this.shortagesTasks = _shortagesTasks;
            this.staffTasks = staffTasks;
        }

        /// <summary>
        /// Get a List of Shortages 
        /// </summary>
        /// <returns></returns>
        public List<DA_ShortagesExtModel> GetShortages(DBInfoModel dbInfo)
        {
            return shortagesTasks.GetShortages(dbInfo);
        }

        /// <summary>
        /// Get a List of Shortages for a store based on staffId. 
        /// Κλήση από το κατάστημα. 
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="staffId">staff.Id (a virtual staff assigned to a store)</param>
        /// <returns></returns>
        public List<DA_ShortagesExtModel> GetShortagesByStore(DBInfoModel dbInfo, long staffId)
        {

            //1. Get store Id (if there is no store then throw exception)
            long daStoreId = staffTasks.GetDaStore(dbInfo, staffId);
            if (daStoreId == 0) throw new BusinessException(Symposium.Resources.Errors.STAFFNOASSIGNED);

            //2. get Shortages By Store
            return shortagesTasks.GetShortagesByStore(dbInfo, daStoreId);
        }


        /// <summary>
        /// Get Shortage by id
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="Id">DA_ShortageProds.Id</param>
        /// <returns></returns>
        public DA_ShortagesExtModel GetShortage(DBInfoModel dbInfo, int Id)
        {
            return shortagesTasks.GetShortage(dbInfo, Id);
        }


        /// <summary>
        /// Insert new Shortage 
        /// </summary>
        /// <param name="Store">DB info</param>
        /// <param name="dbInfo">DA_ShortageProdsModel to insert</param>
        /// <param name="staffId">staff.Id (a virtual staff assigned to a store).
        ///  call from BackOffice:  model.StoreId != 0, staffId = anything; 
        ///  call From DA:          model.StoreId != 0, staffId = anything; 
        ///  call from local store: model.StoreId == 0, staffId > 0; 
        /// </param>
        /// <returns></returns>
        public long Insert(DBInfoModel Store, DA_ShortageProdsModel dbInfo, long staffId=0)
        {
            //1. Get store Id (if there is no store then throw exception)
            if (staffId != 0 && dbInfo.StoreId==0)
            {
                long daStoreId = staffTasks.GetDaStore(Store, staffId);
                if (daStoreId == 0) throw new BusinessException(Symposium.Resources.Errors.STAFFNOASSIGNED);
                dbInfo.StoreId = daStoreId;
            }

            //2. insert
            return shortagesTasks.Insert(Store, dbInfo);
        }

        /// <summary>
        /// Delete a Shortage by id
        /// </summary>
        /// <param name="Store">DB info</param>
        /// <param name="Id">DA_ShortageProds.Id</param>
        /// <returns>return num of records affected</returns>
        public int Delete(DBInfoModel Store, int Id)
        {
            return shortagesTasks.Delete(Store, Id);
        }


        /// <summary>
        /// Delete all temporary Shortages for a store
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="staffId">staff.Id (a virtual staff assigned to a store)</param>
        /// <returns>return num of records affected</returns>
        public int DeleteTemp(DBInfoModel dbInfo, long staffId)
        {
            //1. Get store Id (if there is no store then throw exception)
            long daStoreId = staffTasks.GetDaStore(dbInfo, staffId);
            if (daStoreId == 0) throw new BusinessException(Symposium.Resources.Errors.STAFFNOASSIGNED);

            //2. delete
            return shortagesTasks.DeleteTemp(dbInfo, daStoreId);
        }

        public long Update(DBInfoModel Store, DA_ShortageProdsModel dbInfo, long staffId = 0)
        {
            if (staffId != 0 && dbInfo.StoreId == 0)
            {
                long daStoreId = staffTasks.GetDaStore(Store, staffId);
                if (daStoreId == 0) throw new BusinessException(Symposium.Resources.Errors.STAFFNOASSIGNED);
                dbInfo.StoreId = daStoreId;
            }

            //2. insert
            return shortagesTasks.Update(Store, dbInfo);
            
        }
    }
}
