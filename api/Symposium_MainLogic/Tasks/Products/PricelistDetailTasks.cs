using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.WebApi.DataAccess.Interfaces.DT;
using Symposium.WebApi.MainLogic.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks
{
    public class PricelistDetailTasks : IPricelistDetailTasks
    {
        IPricelistDetailDT pricelistDetailDT;
        public PricelistDetailTasks(IPricelistDetailDT pricelistDetailDT)
        {
            this.pricelistDetailDT = pricelistDetailDT;
        }

        /// <summary>
        /// Updates pricelist detail with new price for specific product and pricelist
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="productId"> Id of product </param>
        /// <param name="pricelistId"> Id of pricelist </param>
        /// <param name="newPrice"> New price value </param>
        /// <returns> Pricelist detail model </returns>
        public PricelistDetailModel UpadteNewPriceForSpecificProductAndPricelist(DBInfoModel Store, long productId, long pricelistId, double newPrice)
        {
            PricelistDetailModel pricelistDetail = pricelistDetailDT.SelectPricelistDetailForProductAndPricelist(Store, productId, pricelistId);
            pricelistDetail.Price = newPrice;
            return pricelistDetailDT.UpdatePricelistDetail(Store, pricelistDetail);
        }
        public PricelistDetailModel UpdateExtraPrice(DBInfoModel dbInfo, long IngredientId, long PriceListDetailId, double newPrice)
        {
            PricelistDetailModel pricelistDetail = pricelistDetailDT.UpdateExtraPrice(dbInfo, IngredientId, PriceListDetailId, newPrice);
            pricelistDetail.Price = newPrice;
            return pricelistDetailDT.UpdatePricelistDetail(dbInfo, pricelistDetail);
        }
        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<PriceListDetailSched_Model> model)
        {
            List<PriceListDetailSched_Model> Upserts = model.Where(w => w.Action != 1).Select(s => s).ToList();
            List<PriceListDetailSched_Model> Deleted = model.Where(w => w.Action == 1).Select(s => s).ToList();

            UpsertListResultModel ups = pricelistDetailDT.InformTablesFromDAServer(Store, Upserts);
            UpsertListResultModel del = pricelistDetailDT.DeleteRecordsSendedFromDAServer(Store, Deleted);

            ups.TotalDeleted += del.TotalDeleted;
            ups.TotalFailed += del.TotalFailed;
            ups.TotalInserted += del.TotalInserted;
            ups.TotalRecords += del.TotalRecords;
            ups.TotalSucceded += del.TotalSucceded;
            ups.TotalUpdated += del.TotalUpdated;
            ups.TotalUpdated += del.TotalUpdated;
            if (ups.Results != null && ups.Results.Count > 0)
                ups.Results.Union(del.Results);
            else
                ups.Results.AddRange(del.Results);

            return ups;
        }

        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel Store, PricelistDetailModel item)
        {
            return pricelistDetailDT.UpdateModel(Store, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel Store, List<PricelistDetailModel> item)
        {
            return pricelistDetailDT.UpdateModelList(Store, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel Store, PricelistDetailModel item)
        {
            return pricelistDetailDT.InsertModel(Store, item);
        }

        /// <summary>
        /// Selects pricelist detail for specific product and specific pricelist
        /// </summary>
        /// <param name="Store"> Store </param>
        /// <param name="productId"> Id of product </param>
        /// <param name="pricelistId"> Id of pricelist </param>
        /// <returns> Pricelist details model </returns>
        public PricelistDetailModel SelectPricelistDetailForProductAndPricelist(DBInfoModel Store, long productId, long pricelistId,
            IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            return pricelistDetailDT.SelectPricelistDetailForProductAndPricelist(Store, productId, pricelistId, dbTran, dbTransact);
        }
    }
}
