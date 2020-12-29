using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.DeliveryAgent
{
    public interface IDA_ShortagesDT
    {
        /// <summary>
        /// Get a List of Shortages 
        /// </summary>
        /// <param name="Store">DB info</param>
        /// <returns></returns>
        List<DA_ShortagesExtModel> GetShortages(DBInfoModel Store);


        /// <summary>
        /// Get a List of Shortages for a store
        /// </summary>
        /// <param name="Store">DB info</param>
        /// <param name="StoreId">Store Id</param>
        /// <returns></returns>
        List<DA_ShortagesExtModel> GetShortagesByStore(DBInfoModel Store, long StoreId);


        /// <summary>
        /// Get Shortage by id
        /// </summary>
        /// <param name="Store">DB info</param>
        /// <param name="Id">DA_ShortageProds.Id</param>
        /// <returns></returns>
        DA_ShortagesExtModel GetShortage(DBInfoModel Store, int Id);


        /// <summary>
        /// Insert new Shortage 
        /// </summary>
        /// <param name="Store">DB info</param>
        /// <param name="model">DA_ShortageProdsModel to insert</param>
        /// <returns></returns>
        long Insert(DBInfoModel Store, DA_ShortageProdsModel model);

        /// <summary>
        /// Delete a Shortage by id
        /// </summary>
        /// <param name="Store">DB info</param>
        /// <param name="Id">DA_ShortageProds.Id</param>
        /// <returns>return num of records affected</returns>
        int Delete(DBInfoModel Store, int Id);


        /// <summary>
        /// Delete all temporary Shortages for a store
        /// </summary>
        /// <param name="Store">DB info</param>
        /// <param name="Id">Store Id</param>
        /// <returns>return num of records affected</returns>
        int DeleteTemp(DBInfoModel Store, long StoreId);

         long Update(DBInfoModel dbInfo, DA_ShortageProdsModel Model);


    }
}
