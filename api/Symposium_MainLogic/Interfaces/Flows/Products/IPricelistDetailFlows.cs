using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows
{
    public interface IPricelistDetailFlows
    {

        /// <summary>
        /// Updates pricelist detail of specific product and pricelist with new price
        /// </summary>
        /// <param name="store"> Store </param>
        /// <param name="productId"> Id of product </param>
        /// <param name="pricelistId"> Id of pricelist </param>
        /// <param name="newPrice"> New price value </param>
        /// <returns> Pricelist detail model </returns>
        PricelistDetailModel UpadteNewPriceForSpecificProductAndPricelist(DBInfoModel store, long productId, long pricelistId, double newPrice);

        PricelistDetailModel UpdateExtraPrice(DBInfoModel dbInfo, long IngredientId, long PriceListDetailId, double newPrice);
        /// <summary>
        /// Return's list of Price Lists Details after upsert, using as searc field DAId
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
    }
}
