using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.DeliveryAgent
{
    public interface IDA_ShortagesFlows
    {
        /// <summary>
        /// Get a List of Shortages 
        /// </summary>
        /// <returns></returns>
        List<DA_ShortagesExtModel> GetShortages(DBInfoModel dbInfo);


        /// <summary>
        /// Get a List of Shortages for a store based on staffId. 
        /// Κλήση από το κατάστημα. 
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="staffId">staff.Id (a virtual staff assigned to a store)</param>
        /// <returns></returns>
        List<DA_ShortagesExtModel> GetShortagesByStore(DBInfoModel dbInfo, long staffId);


        /// <summary>
        /// Get Shortage by id
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="Id">DA_ShortageProds.Id</param>
        /// <returns></returns>
        DA_ShortagesExtModel GetShortage(DBInfoModel dbInfo, int Id);


        /// <summary>
        /// Insert new Shortage 
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="model">DA_ShortageProdsModel to insert</param>
        /// <param name="staffId">staff.Id (a virtual staff assigned to a store).
        ///   call from BackOffice:  model.StoreId != 0, staffId = anything; 
        ///   call From DA:          model.StoreId != 0, staffId = anything; 
        ///   call from local store: model.StoreId == 0, staffId > 0; 
        /// </param>
        /// <returns></returns>
        long Insert(DBInfoModel dbInfo, DA_ShortageProdsModel model, long staffId = 0);

        long Update(DBInfoModel dbInfo, DA_ShortageProdsModel model, long staffId = 0);


        /// <summary>
        /// Delete a Shortage by id
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="Id">DA_ShortageProds.Id</param>
        /// <returns>return num of records affected</returns>
        int Delete(DBInfoModel dbInfo, int Id);


        /// <summary>
        /// Delete all temporary Shortages for a store
        /// </summary>
        /// <param name="dbInfo">DB info</param>
        /// <param name="Id">Store Id</param>
        /// <returns>return num of records affected</returns>
        int DeleteTemp(DBInfoModel dbInfo, long StoreId);

    }
}
