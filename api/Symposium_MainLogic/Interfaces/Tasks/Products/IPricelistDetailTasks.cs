using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks
{
    public interface IPricelistDetailTasks
    {

        /// <summary>
        /// Updates pricelist detail with new price for specific product and pricelist
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="productId"> Id of product </param>
        /// <param name="pricelistId"> Id of pricelist </param>
        /// <param name="newPrice"> New price value </param>
        /// <returns> Pricelist detail model </returns>
        PricelistDetailModel UpadteNewPriceForSpecificProductAndPricelist(DBInfoModel Store, long productId, long pricelistId, double newPrice);

        PricelistDetailModel UpdateExtraPrice(DBInfoModel dbInfo, long IngredientId, long PriceListDetailId, double newPrice);

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PriceListDetailSched_Model> model);

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModel(DBInfoModel Store, PricelistDetailModel item);

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int UpdateModelList(DBInfoModel Store, List<PricelistDetailModel> item);

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        long InsertModel(DBInfoModel Store, PricelistDetailModel item);

        /// <summary>
        /// Selects pricelist detail for specific product and specific pricelist
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="productId"> Id of product </param>
        /// <param name="pricelistId"> Id of pricelist </param>
        /// <returns> Pricelist details model </returns>
        PricelistDetailModel SelectPricelistDetailForProductAndPricelist(DBInfoModel Store, long productId, long pricelistId, 
            IDbConnection dbTran = null, IDbTransaction dbTransact = null);

    }
}
