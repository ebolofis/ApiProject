using Symposium.Helpers;
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

namespace Symposium.WebApi.MainLogic.Tasks {
    public class VatTasks : IVatTasks
    {
        IVatDT dt;
        IPricelistDT pricelistDT;

        public VatTasks(IVatDT dt, IPricelistDT pricelistDT)
        {
            this.dt = dt;
            this.pricelistDT=pricelistDT;
    }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesFromDAServer(DBInfoModel Store, List<VatSched_Model> model)
        {
            List<VatSched_Model> Upserts = model.Where(w => w.Action != 1).Select(s => s).ToList();
            List<VatSched_Model> Deleted = model.Where(w => w.Action == 1).Select(s => s).ToList();

            UpsertListResultModel ups = dt.InformTablesFromDAServer(Store, Upserts);
            UpsertListResultModel del = dt.DeleteRecordsSendedFromDAServer(Store, Deleted);

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
        /// Return a list with all vats Included deleted
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public List<VatModel> GetAllVats(DBInfoModel dbInfo, IDbConnection dbTran = null, IDbTransaction dbTransact = null)
        {
            return dt.GetAllVats(dbInfo, dbTran, dbTransact);
        }

        /// <summary>
        /// return the Vat model of a product and a given price-list
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public VatModel GetProductVatFromPricelist(DBInfoModel dbInfo,long productID, long priceListId)
        {
            PricelistExtModel prsl = pricelistDT.GetProductFromPriceList(dbInfo, productID, priceListId);
            if (prsl == null || prsl.Details == null || prsl.Details.Count == 0)
            {
                throw new BusinessException("Product with Id: " + productID.ToString() + " not found in Price-List: " + priceListId.ToString());
            }
            return GetAllVats(dbInfo).FirstOrDefault(x => x.Id == prsl.Details[0].VatId);
        }

        /// <summary>
        /// return the Vat model of an extra/ingredient and a given price-list
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public VatModel GetExtraVatFromPricelist(DBInfoModel dbInfo, long extraID, long priceListId)
        {
            PricelistExtModel prsl = pricelistDT.GetExtraFromPriceList(dbInfo, extraID, priceListId);
            if (prsl == null || prsl.Details == null || prsl.Details.Count == 0)
            {
                throw new BusinessException("Extra with Id: " + extraID.ToString() + " not found in Price-List: " + priceListId.ToString());
            }
            return GetAllVats(dbInfo).FirstOrDefault(x => x.Id == prsl.Details[0].VatId);
        }


        /// <summary>
        /// update unit model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModel(DBInfoModel dbInfo, VatModel item)
        {
            return dt.UpdateModel(dbInfo, item);
        }

        /// <summary>
        /// update unit list model
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateModelList(DBInfoModel dbInfo, List<VatModel> item)
        {
            return dt.UpdateModelList(dbInfo, item);
        }

        /// <summary>
        /// Insert new unit on db
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long InsertModel(DBInfoModel dbInfo, VatModel item)
        {
            return dt.InsertModel(dbInfo, item);
        }
    }
}
