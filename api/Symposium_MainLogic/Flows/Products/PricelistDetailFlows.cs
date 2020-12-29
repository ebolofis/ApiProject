using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.MainLogic.Interfaces.Flows;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows
{
    public class PricelistDetailFlows : IPricelistDetailFlows
    {
        IPricelistDetailTasks pricelistDetailTask;

        public PricelistDetailFlows(IPricelistDetailTasks pricelistDetailTask)
        {
            this.pricelistDetailTask = pricelistDetailTask;
        }


        /// <summary>
        /// Updates price-list detail of specific product and price-list with new price
        /// </summary>
        /// <param name="dbInfo"> Store </param>
        /// <param name="productId"> Id of product </param>
        /// <param name="pricelistId"> Id of price-list </param>
        /// <param name="newPrice"> New price value </param>
        /// <returns> Price-list detail model </returns>
        public PricelistDetailModel UpadteNewPriceForSpecificProductAndPricelist(DBInfoModel dbInfo, long productId, long pricelistId, double newPrice)
        {
            return pricelistDetailTask.UpadteNewPriceForSpecificProductAndPricelist(dbInfo, productId, pricelistId, newPrice);
        }

        /// <summary>
        /// Updates pricelist detail of specific extra detail and specific pricelist with new price
        /// </summary>
        /// <param name="IngredientId"> Id of product </param>
        /// <param name="PricelistDetailId"> Id of pricelist </param>
        /// <param name="Price"> New price value </param>
        /// <returns> Updated object </returns>
        public PricelistDetailModel UpdateExtraPrice(DBInfoModel dbInfo, long IngredientId, long PriceListDetailId, double newPrice)
        {
            return pricelistDetailTask.UpdateExtraPrice(dbInfo, IngredientId, PriceListDetailId, newPrice);
        }

        /// <summary>
        /// Return's list of actions after upsert, using as search field DAId
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel dbInfo, List<PriceListDetailSched_Model> model)
        {
            return pricelistDetailTask.InformTablesFromDAServer(dbInfo, model);
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel dbInfo, PricelistDetailModel item)
        {
            return pricelistDetailTask.UpdateModel(dbInfo, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel dbInfo, List<PricelistDetailModel> item)
        {
            return pricelistDetailTask.UpdateModelList(dbInfo, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel dbInfo, PricelistDetailModel item)
        {
            return pricelistDetailTask.InsertModel(dbInfo, item);
        }
    }
}
