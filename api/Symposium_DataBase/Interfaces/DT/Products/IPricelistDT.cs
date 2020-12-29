using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT
{
    public interface IPricelistDT
    {

        /// <summary>
        /// Return the extended price-lists (active only). Every price-list contains the list of Details.
        /// </summary>
        /// <returns></returns>
        List<PricelistExtModel> GetExtentedList(DBInfoModel Store);

        /// <summary>
        /// Return prices info for specific product and price-list (active only)
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="ProductId">Product Id</param>
        /// <param name="PriceListId">PriceList Id</param>
        /// <returns></returns>
        PricelistExtModel GetProductFromPriceList(DBInfoModel dbInfo, long ProductId, long PriceListId);

        /// <summary>
        /// Return prices info for specific extra/Ingredient and price-list (active only)
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="IngredientId">Ingredient Id</param>
        /// <param name="PriceListId">PriceList Id</param>
        /// <returns></returns>
        PricelistExtModel GetExtraFromPriceList(DBInfoModel dbInfo, long IngredientId, long PriceListId);

        /// <summary>
        /// Return's list of actions after upsert, using as search field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PriceListSched_Model> model);

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModel(DBInfoModel Store, PricelistModel item);

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModelList(DBInfoModel Store, List<PricelistModel> item);

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long InsertModel(DBInfoModel Store, PricelistModel item);

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel DeleteRecordsSendedFromDAServer(DBInfoModel Store, List<PriceListSched_Model> model);

        /// <summary>
        /// Return Table Id from field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="dAId"></param>
        /// <returns></returns>
        long? GetIdByDAIs(DBInfoModel Store, long dAId, IDbConnection dbTran = null, IDbTransaction dbTransact = null);
    }
}
