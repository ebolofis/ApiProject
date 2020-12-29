using Symposium.Models.Models;
using Symposium.Models.Models.DeliveryAgent;
using Symposium.Models.Models.Promos;
using Symposium.WebApi.MainLogic.Interfaces.Flows.Promos;
using Symposium.WebApi.MainLogic.Interfaces.Tasks.Promos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symposium.WebApi.MainLogic.Flows.Promos
{
    public class PromotionsFlows : IPromotionsFlows
    {
        IPromotionsTasks PromotionsTasks;
        public PromotionsFlows(IPromotionsTasks promotionsTasks)
        {
            this.PromotionsTasks = promotionsTasks;
        }

        /// <summary>
        /// Get Promotions Model (PromotionsHeader, PromotionsCombos, PromotionsDiscounts)
        /// </summary>
        /// <returns>
        /// PromotionsModels
        /// </returns>
        public List<PromotionsModels> GetAll(DBInfoModel dbinfo)
        {
            List<PromotionsModels> promoModelList = new List<PromotionsModels>();
            //1. Get Promotions Header List
            promoModelList = PromotionsTasks.GetPromotionsHeader(dbinfo);
            if (promoModelList != null)
            {
                foreach (PromotionsModels header in promoModelList)
                {
                    //2. Get Promotions Combos List
                    header.PromoCombo = PromotionsTasks.GetPromotionsComboById(dbinfo, header.Id);
                    //3. Get Promotions Discounts List
                    header.PromoDiscounts = PromotionsTasks.GetPromotionsDiscountsById(dbinfo, header.Id);
                }
            }
            return promoModelList;
        }

        /// <summary>
        /// Get Promotions Model By HeaderId (PromotionsHeader, PromotionsCombos, PromotionsDiscounts)
        /// </summary>
        /// <param name="Id">HeaderId</param>
        /// <returns>
        /// PromotionsModels
        /// </returns>
        public PromotionsModels GetPromotions(DBInfoModel dbinfo, long Id)
        {
            PromotionsModels promoModel = new PromotionsModels();
            //1. Get Promotions Header
            promoModel = PromotionsTasks.GetPromotionsHeaderById(dbinfo, Id);
            if (promoModel != null)
            {
                //2. Get Promotions Combos List
                promoModel.PromoCombo = PromotionsTasks.GetPromotionsComboById(dbinfo, Id);
                //3. Get Promotions Discounts List
                promoModel.PromoDiscounts = PromotionsTasks.GetPromotionsDiscountsById(dbinfo, Id);
            }
            return promoModel;
        }

        /// <summary>
        /// Insert Promotions Model (PromotionsHeader, PromotionsCombos and PromotionsDiscounts).
        /// </summary>
        /// <param name="PromotionsModels">Promotions Models</param>
        /// <returns>
        /// HeaderId
        /// </returns>
        public long InsertPromotion(DBInfoModel dbinfo, PromotionsModels Model)
        {
            return PromotionsTasks.InsertPromotion(dbinfo, Model);
        }

        public long InsertCombo(DBInfoModel dbinfo, PromotionsCombos Model)
        {
            return PromotionsTasks.InsertCombo(dbinfo, Model);
        }

        public long InsertDiscount(DBInfoModel dbinfo, PromotionsDiscounts Model)
        {
            return PromotionsTasks.InsertDiscount(dbinfo, Model);
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
            return PromotionsTasks.UpdatePromotion(dbinfo, Model);
        }

        public PromotionsCombos UpdateCombo(DBInfoModel dbinfo, PromotionsCombos Model)
        {
            return PromotionsTasks.UpdateCombo(dbinfo, Model);

        }
        public PromotionsDiscounts UpdateDiscount(DBInfoModel dbinfo, PromotionsDiscounts Model)
        {
            return PromotionsTasks.UpdateDiscount(dbinfo, Model);
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
            return PromotionsTasks.DeletePromotion(dbinfo, Id);
        }



        public long DeleteCombo(DBInfoModel dbinfo, long Id)
        {
            return PromotionsTasks.DeleteCombo(dbinfo, Id);
        }

        public long DeleteDiscount(DBInfoModel dbinfo, long Id)
        {
            return PromotionsTasks.DeleteDiscount(dbinfo, Id);
        }


        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId For Promotion Header
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesForPromotionHeaderFromDAServer(DBInfoModel dbInfo, List<PromotionsHeaderSched_Model> model)
        {
            return PromotionsTasks.InformTablesForPromotionHeaderFromDAServer(dbInfo, model);
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId For Promotions Combos
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesForPromotionCombosFromDAServer(DBInfoModel dbInfo, List<PromotionsCombosSched_Model> model)
        {
            return PromotionsTasks.InformTablesForPromotionCombosFromDAServer(dbInfo, model);
        }

        /// <summary>
        /// Return's list of actions after upsert, using as searc field DAId Promotions Descounts
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public UpsertListResultModel InformTablesForPromotionDiscountFromDAServer(DBInfoModel dbInfo, List<PromotionsDiscountsSched_Model> model)
        {
            return PromotionsTasks.InformTablesForPromotionDiscountFromDAServer(dbInfo, model);
        }

        /// <summary>
        /// Get Promotions Pricelists
        /// </summary>
        /// <returns>PromotionsPricelistModel</returns>
        public List<PromotionsPricelistModel> GetPromotionsPricelists(DBInfoModel dbinfo)
        {
            return PromotionsTasks.GetPromotionsPricelists(dbinfo);
        }

        /// <summary>
        /// Upsert Promotions Pricelists On Save
        /// </summary>
        /// <returns>PromotionsPricelistModel</returns>
        public List<PromotionsPricelistModel> UpsertPromotionPricelists(DBInfoModel dbinfo, List<PromotionsPricelistModel> Model)
        {
            return PromotionsTasks.UpsertPromotionPricelists(dbinfo, Model);
        }

    }


}
