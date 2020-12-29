using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Promos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Tasks.Promos
{
    public interface IPromotionsTasks
    {
        /// <summary>
        /// Get Promotions Header Only
        /// </summary>
        /// <returns>
        /// PromotionsModels List
        /// </returns>
        List<PromotionsModels> GetPromotionsHeader(DBInfoModel dbinfo);

        /// <summary>
        /// Get Promotions Header Only By HeaderId
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// PromotionsModels
        /// </returns>
        PromotionsModels GetPromotionsHeaderById(DBInfoModel dbinfo, long Id);

        /// <summary>
        /// Get List of Promotions Combo By HeaderId
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// List of PromotionsCombos Extended Model
        /// </returns>
        List<PromotionsCombosExt> GetPromotionsComboById(DBInfoModel dbinfo, long Id);

        /// <summary>
        /// Get List of Promotions Discounts By HeaderId
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// List of Promotions Discounts Extended Model
        /// </returns>
        List<PromotionsDiscountsExt> GetPromotionsDiscountsById(DBInfoModel dbinfo, long Id);

        /// <summary>
        /// Insert Promotions Model (PromotionsHeader, PromotionsCombos and PromotionsDiscounts).
        /// </summary>
        /// <param name="PromotionsModels">Promotions Models</param>
        /// <returns>
        /// HeaderId
        /// </returns>
        long InsertPromotion(DBInfoModel dbinfo, PromotionsModels Model);

        long InsertCombo(DBInfoModel dbinfo, PromotionsCombos Model);

        long InsertDiscount(DBInfoModel dbinfo, PromotionsDiscounts Model);

        /// <summary>
        /// Update Promotions Model (PromotionsHeader, PromotionsCombos and PromotionsDiscounts).
        /// </summary>
        /// <param name="PromotionsModels">Promotions Models</param>
        /// <returns>
        /// HeaderId
        /// </returns>
        long UpdatePromotion(DBInfoModel dbinfo, PromotionsModels Model);

        PromotionsCombos UpdateCombo(DBInfoModel dbinfo, PromotionsCombos Model);

        PromotionsDiscounts UpdateDiscount(DBInfoModel dbinfo, PromotionsDiscounts Model);
        /// <summary>
        /// Delete Promotions Model (PromotionsHeader, PromotionsCombos and PromotionsDiscounts).
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// HeaderId
        /// </returns>
        long DeletePromotion(DBInfoModel dbinfo, long Id);
        long DeleteCombo(DBInfoModel dbinfo, long Id);
        long DeleteDiscount(DBInfoModel dbinfo, long Id);

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId From Promotion Headers
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesForPromotionHeaderFromDAServer(DBInfoModel Store, List<PromotionsHeaderSched_Model> model);

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId For Promotion Combos
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesForPromotionCombosFromDAServer(DBInfoModel Store, List<PromotionsCombosSched_Model> model);

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId For Promotion Discount
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesForPromotionDiscountFromDAServer(DBInfoModel Store, List<PromotionsDiscountsSched_Model> model);

        /// <summary>
        /// Get Promotions Pricelists
        /// </summary>
        /// <returns>PromotionsPricelistModel</returns>
        List<PromotionsPricelistModel> GetPromotionsPricelists(DBInfoModel dbinfo);

        /// <summary>
        /// Upsert Promotions Pricelists On Save
        /// </summary>
        /// <returns>PromotionsPricelistModel</returns>
        List<PromotionsPricelistModel> UpsertPromotionPricelists(DBInfoModel dbinfo, List<PromotionsPricelistModel> Model);
    }
}
