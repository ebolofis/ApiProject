using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Promos;
using Symposium.WebApi.DataAccess.Interfaces.DT.Promos;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Promos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Tasks.Promos
{
    public class PromotionsTasks : IPromotionsTasks
    {
        IPromotionsDT PromotionsDT;
        public PromotionsTasks(IPromotionsDT promotionsDT)
        {
            this.PromotionsDT = promotionsDT;
        }

        /// <summary>
        /// Get Promotions Header Only
        /// </summary>
        /// <returns>
        /// PromotionsModels
        /// </returns>
        public List<PromotionsModels> GetPromotionsHeader(DBInfoModel dbinfo)
        {
            return PromotionsDT.GetPromotionsHeader(dbinfo);
        }

        /// <summary>
        /// Get Promotions Header Only By HeaderId
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// PromotionsModels
        /// </returns>
        public PromotionsModels GetPromotionsHeaderById(DBInfoModel dbinfo, long Id)
        {
            return PromotionsDT.GetPromotionsHeaderById(dbinfo, Id);
        }

        /// <summary>
        /// Get List of Promotions Combo By HeaderId
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// List of PromotionsCombos
        /// </returns>
        public List<PromotionsCombosExt> GetPromotionsComboById(DBInfoModel dbinfo, long Id)
        {
            return PromotionsDT.GetPromotionsComboById(dbinfo, Id);
        }

        /// <summary>
        /// Get List of Promotions Discounts By HeaderId
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// List of Promotions Discounts Extended Model
        /// </returns>
        public List<PromotionsDiscountsExt> GetPromotionsDiscountsById(DBInfoModel dbinfo, long Id)
        {
            return PromotionsDT.GetPromotionsDiscountsById(dbinfo, Id);
        }

        /// <summary>
        /// Insert Promotions Header Only
        /// </summary>
        /// <returns>
        /// Headers Id
        /// </returns>
        public long InsertPromotion(DBInfoModel dbinfo, PromotionsModels Model)
        {
            return PromotionsDT.InsertPromotion(dbinfo, Model);
        }

        public long InsertCombo(DBInfoModel dbinfo, PromotionsCombos Model)
        {
            return PromotionsDT.InsertCombo(dbinfo, Model);
        }
        public long InsertDiscount(DBInfoModel dbinfo, PromotionsDiscounts Model)
        {
            return PromotionsDT.InsertDiscount(dbinfo, Model);
        }
        /// <summary>
        /// Update Promotions Model (PromotionsHeader, PromotionsCombos and PromotionsDiscounts).
        /// </summary>
        /// <param name="PromotionsModels">Promotions Models</param>
        /// <returns>
        /// HeaderId
        /// </returns>
        public long UpdatePromotion(DBInfoModel dbinfo, PromotionsModels Model)
        {
            return PromotionsDT.UpdatePromotion(dbinfo, Model);
        }
        public PromotionsCombos UpdateCombo(DBInfoModel dbinfo, PromotionsCombos Model)
        {
            return PromotionsDT.UpdateCombo(dbinfo, Model);
        }

        public PromotionsDiscounts UpdateDiscount(DBInfoModel dbinfo, PromotionsDiscounts Model)
        {
            return PromotionsDT.UpdateDiscount(dbinfo, Model);
        }
        /// <summary>
        /// Delete Promotions Model (PromotionsHeader, PromotionsCombos and PromotionsDiscounts).
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// HeaderId
        /// </returns>
        public long DeletePromotion(DBInfoModel dbinfo, long Id)
        {
            return PromotionsDT.DeletePromotion(dbinfo, Id);
        }


        public long DeleteCombo(DBInfoModel dbinfo, long Id)
        {
            return PromotionsDT.DeleteCombo(dbinfo, Id);
        }

        public long DeleteDiscount(DBInfoModel dbinfo, long Id)
        {
            return PromotionsDT.DeleteDiscount(dbinfo, Id);
        }


        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId From Promotion Headers
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesForPromotionHeaderFromDAServer(DBInfoModel Store, List<PromotionsHeaderSched_Model> model)
        {
            List<PromotionsHeaderSched_Model> Upserts = model.Where(w => w.Action != 1).Select(s => s).ToList();
            List<PromotionsHeaderSched_Model> Deleted = model.Where(w => w.Action == 1).Select(s => s).ToList();

            UpsertListResultModel ups = PromotionsDT.InformPromotionHeaderTablesFromDAServer(Store, Upserts);
            UpsertListResultModel del = PromotionsDT.DeleteRecordsForPromitonHeaderSendedFromDAServer(Store, Deleted);

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
        /// Return's list of actions after upsert, using as searc field DAId For Promotion Combos
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesForPromotionCombosFromDAServer(DBInfoModel Store, List<PromotionsCombosSched_Model> model)
        {
            List<PromotionsCombosSched_Model> Upserts = model.Where(w => w.Action != 1).Select(s => s).ToList();
            List<PromotionsCombosSched_Model> Deleted = model.Where(w => w.Action == 1).Select(s => s).ToList();

            UpsertListResultModel ups = PromotionsDT.InformPromotionCombosTablesFromDAServer(Store, Upserts);
            UpsertListResultModel del = PromotionsDT.DeleteRecordsForPromotionCombosSendedFromDAServer(Store, Deleted);

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
        /// Return's list of actions after upsert, using as searc field DAId For Promotion Discount
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesForPromotionDiscountFromDAServer(DBInfoModel Store, List<PromotionsDiscountsSched_Model> model)
        {
            List<PromotionsDiscountsSched_Model> Upserts = model.Where(w => w.Action != 1).Select(s => s).ToList();
            List<PromotionsDiscountsSched_Model> Deleted = model.Where(w => w.Action == 1).Select(s => s).ToList();

            UpsertListResultModel ups = PromotionsDT.InformPromotionDiscountTablesFromDAServer(Store, Upserts);
            UpsertListResultModel del = PromotionsDT.DeleteRecordsForPromotionDiscountsSendedFromDAServer(Store, Deleted);

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
        /// Get Promotions Pricelists
        /// </summary>
        /// <returns>PromotionsPricelistModel</returns>
        public List<PromotionsPricelistModel> GetPromotionsPricelists(DBInfoModel dbinfo)
        {
            return PromotionsDT.GetPromotionsPricelists(dbinfo);
        }

        /// <summary>
        /// Upsert Promotions Pricelists On Save
        /// </summary>
        /// <returns>PromotionsPricelistModel</returns>
        public List<PromotionsPricelistModel> UpsertPromotionPricelists(DBInfoModel dbinfo, List<PromotionsPricelistModel> Model)
        {
            List<PromotionsPricelistModel> NewModel = new List<PromotionsPricelistModel>();
            //Delete All
            PromotionsDT.DeleteAllPromotionPricelists(dbinfo);

            //Insert New PromotionsPricelistModel
            foreach (PromotionsPricelistModel promoPricelist in Model)
            {
                PromotionsDT.UpsertPromotionPricelists(dbinfo, promoPricelist);
            }

            //Return Updated PromotionsPricelistModel
            NewModel = GetPromotionsPricelists(dbinfo);
            return NewModel;
        }

    }
}
