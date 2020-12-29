using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Promos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.DataAccess.Interfaces.DT.Promos
{
    public interface IPromotionsDT
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
        /// Update or Insert's new records from Delivery Agent Tables for PromotionHeader
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformPromotionHeaderTablesFromDAServer(DBInfoModel Store, List<PromotionsHeaderSched_Model> model);

        /// <summary>
        /// Update or Insert's new records from Delivery Agent Tables for PromotionCombos
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformPromotionCombosTablesFromDAServer(DBInfoModel Store, List<PromotionsCombosSched_Model> model);

        /// <summary>
        /// Update or Insert's new records from Delivery Agent Tables for PromotionDiscount
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel InformPromotionDiscountTablesFromDAServer(DBInfoModel Store, List<PromotionsDiscountsSched_Model> model);

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel DeleteRecordsForPromitonHeaderSendedFromDAServer(DBInfoModel Store, List<PromotionsHeaderSched_Model> model);

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel DeleteRecordsForPromotionCombosSendedFromDAServer(DBInfoModel Store, List<PromotionsCombosSched_Model> model);

        /// <summary>
        /// Delete's Or Set's the Field IsDeleted if Exists to true for a list
        /// </summary>
        /// <param name="Store"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        UpsertListResultModel DeleteRecordsForPromotionDiscountsSendedFromDAServer(DBInfoModel Store, List<PromotionsDiscountsSched_Model> model);

        /// <summary>
        /// Get Promotions Pricelists
        /// </summary>
        /// <returns>PromotionsPricelistModel</returns>
        List<PromotionsPricelistModel> GetPromotionsPricelists(DBInfoModel dbinfo);

        /// <summary>
        /// Delete ALL Promotions Pricelists and Clear Table
        /// </summary>
        /// <returns></returns>
        void DeleteAllPromotionPricelists(DBInfoModel dbinfo);


        /// <summary>
        /// Upsert Promotions Pricelists On Save
        /// </summary>
        /// <returns></returns>
        void UpsertPromotionPricelists(DBInfoModel dbinfo, PromotionsPricelistModel Model);

    }
}
