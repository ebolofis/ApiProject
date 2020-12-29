using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IPricelistMasterDT
    {
        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PriceListMasterSched_Model> model);

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModel(DBInfoModel Store, PricelistMasterModel item);

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModelList(DBInfoModel Store, List<PricelistMasterModel> item);

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long InsertModel(DBInfoModel Store, PricelistMasterModel item);

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<PriceListMasterSched_Model> model);

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        long? GetIdByDAIs(DBInfoModel Store, long dAId);
    }
}
