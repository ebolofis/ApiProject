using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Promos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Interfaces.Flows.Promos
{
    public interface IPromotionsFlows
    {
        /// <summary>
        /// Get Promotions Model (PromotionsHeader, PromotionsCombos, PromotionsDiscounts)
        /// </summary>
        /// <returns>
        /// PromotionsModels
        /// </returns>
        List<PromotionsModels> GetAll(DBInfoModel dbinfo);

        /// <summary>
        /// Get Promotions Model By HeaderId (PromotionsHeader, PromotionsCombos, PromotionsDiscounts)
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// PromotionsModels
        /// </returns>
        PromotionsModels GetPromotions(DBInfoModel dbinfo, long Id);

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
        /// Return's list of actions after upsert, using as searc field DAId For Promotion Header
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesForPromotionHeaderFromDAServer(DBInfoModel dbInfo, List<PromotionsHeaderSched_Model> model);

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId For Promotions Combos
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesForPromotionCombosFromDAServer(DBInfoModel dbInfo, List<PromotionsCombosSched_Model> model);

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId Promotions Descounts
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformTablesForPromotionDiscountFromDAServer(DBInfoModel dbInfo, List<PromotionsDiscountsSched_Model> model);

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
